﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.EntityFrameworkCore;
using MShare_ASP.API.Request;
using MShare_ASP.API.Response;
using MShare_ASP.Data;
using MShare_ASP.Services.Exceptions;
using MShare_ASP.Utils;

namespace MShare_ASP.Services {
    internal class SpendingService : ISpendingService {
        private MshareDbContext DbContext { get; }
        public SpendingService(MshareDbContext dbContext) {
            DbContext = dbContext;
        }

        public IList<SpendingData> ToSpendingData(IList<DaoSpending> spendings){
            return spendings.Select(x => new SpendingData(){
                Name = x.Name,
                Creditor = new UserData() {
                    Id = x.Creditor.Id,
                    Name = x.Creditor.DisplayName
                },
                MoneyOwed = x.MoneyOwed,
                Debtors = x.Debtors.Select(d => new DebtorData(){
                    Id = d.DebtorUserId,
                    Name = d.Debtor.DisplayName,
                    Debt = d.Debt.Value
                }).ToList()
            }).ToList();
        }

        public IList<OptimisedDebtData> ToOptimisedDebtData(IList<DaoOptimizedDebt> optimizedDebts){
            return optimizedDebts.Select(x => new OptimisedDebtData(){
                DebtorId = x.UserOwesId,
                CreditorId = x.UserOwedId,
                OptimisedDebtAmount = x.OweAmount
            }).ToList();
        }

        public async Task<IList<DaoSpending>> GetSpendingsForGroup(long groupId) {
            return await DbContext.Spendings
                            .Where(x => x.GroupId == groupId)
                            .Include(x => x.Creditor)
                            .Include(x => x.Debtors).ThenInclude(x => x.Debtor)
                            .ToListAsync();
        }

        public async Task<IList<DaoOptimizedDebt>> GetOptimizedDebtForGroup(long userId, long groupId){
            //TODO: Missing security checks 
            await OptimizeSpendingForGroup(groupId);
            return await DbContext.OptimizedDebt.Where(x => x.GroupId == groupId).ToListAsync();
        }

        public async Task OptimizeSpendingForGroup(long groupId){
            var currentGroup = await DbContext.Groups
                                              .Include(x => x.Members)
                                              .SingleOrDefaultAsync(x => x.Id == groupId);
            if (currentGroup == null)
                throw new ResourceGoneException("group_gone");
            var Spendings = await GetSpendingsForGroup(groupId);
            Dictionary<int, long> NumberToId = new Dictionary<int, long>();
            Dictionary<long, int> IdToNumber = new Dictionary<long, int>();
            {
                int i = 0;
                foreach (var MemberId in currentGroup.Members.Select(x => x.UserId))
                {
                    NumberToId.Add(i, MemberId);
                    IdToNumber.Add(MemberId, i);
                    i++;
                }
            }


            int ingroup = NumberToId.Count;
            long[,] owes = new long[ingroup, ingroup];
            for (int i = 0; i < ingroup; i++){
                var iSpending = Spendings.Where(x => x.CreditorUserId == NumberToId[i]);
                foreach(DaoSpending ds in iSpending){
                    foreach(DaoDebtor dd in ds.Debtors)
                    {
                        owes[IdToNumber[dd.DebtorUserId], i] += dd.Debt??0;
                    }
                }
            }
            var Optimizer = new SpendingOptimizer(owes, ingroup);
            Optimizer.Optimize();
            owes = Optimizer.GetResult();
            var oldOptimized = await DbContext.OptimizedDebt.Where(x => x.GroupId == groupId).ToListAsync();
            DbContext.OptimizedDebt.RemoveRange(oldOptimized);
            for(int i = 0; i < ingroup; i++) {
                for(int j = 0; j < ingroup; j++) {
                    if(owes[i,j] > 0){
                        DaoOptimizedDebt optdebt = new DaoOptimizedDebt() {
                           GroupId = groupId,
                           UserOwesId = NumberToId[i],
                           UserOwedId = NumberToId[j],
                           OweAmount = owes[i, j]
                        };
                        await DbContext.OptimizedDebt.AddAsync(optdebt);
                    }
                }
            }
            await DbContext.SaveChangesAsync();
            //save results
        }

        public async Task CreateNewSpending(NewSpending newSpending, long userId){
            var currentUser = await DbContext.Users.FindAsync(userId);
            if (currentUser == null)
                throw new ResourceGoneException("current_user_gone");

            var currentGroup = await DbContext.Groups
                                              .Include(x => x.Members)
                                              .SingleOrDefaultAsync(x => x.Id == newSpending.GroupId);

            if (currentGroup == null)
                throw new ResourceGoneException("group_gone");

            if (newSpending.Debtors.Any(x => newSpending.Debtors.Count(d => d.DebtorId == x.DebtorId) > 1))
                throw new BusinessException("duplicate_debtor_id_found");

            if (!currentGroup.Members.Any(x => x.UserId == userId))
                throw new ResourceForbiddenException("user_not_member");

            if (newSpending.Debtors.Any()
            && !newSpending.Debtors.All(x => currentGroup.Members.Any(m => m.UserId == x.DebtorId)))
                throw new BusinessException("not_all_debtors_are_members");

            if (newSpending.Debtors.Any()
            && !newSpending.Debtors.All(x => DbContext.Users.Find(x.DebtorId) != null))
                throw new ResourceGoneException("debtor_gone");


            DaoSpending spending = new DaoSpending(){
                Name = newSpending.Name,
                MoneyOwed = newSpending.MoneySpent,
                Group = currentGroup,
                GroupId = currentGroup.Id,
                Creditor = currentUser,
                CreditorUserId = currentUser.Id
            };

            var nonSpecifiedCount = newSpending.Debtors.Any()
                ? newSpending.Debtors.Count(s => !s.Debt.HasValue)
                : currentGroup.Members.Count();

            var autoCalculatedIndividualDebt = nonSpecifiedCount == 0
                ? 0
                : (newSpending.MoneySpent - newSpending.Debtors.Sum(x => x.Debt ?? 0)) / nonSpecifiedCount;

            var debtors = newSpending.Debtors.Select(x => new DaoDebtor(){
                Spending = spending,
                DebtorUserId = x.DebtorId,
                Debt = x.Debt ?? autoCalculatedIndividualDebt
            }).ToList();

            // If there were no debtors, populate it from group members
            if (!debtors.Any()){
                debtors = currentGroup.Members.Select(x => new DaoDebtor()
                {
                    Spending = spending,
                    DebtorUserId = x.UserId,
                    Debt = autoCalculatedIndividualDebt
                }).ToList();
            }

            spending.Debtors = debtors.ToList();

            var insertCount = 1 + spending.Debtors.Count;
            await DbContext.Spendings.AddAsync(spending);
            if (await DbContext.SaveChangesAsync() != insertCount)
                throw new DatabaseException("spending_not_inserted");
            
        }

    }
}
