﻿using Microsoft.EntityFrameworkCore;
using MShare_ASP.Utils;

namespace MShare_ASP.Data {
	/// <summary>
	/// Db Context for all data in MShare
	/// </summary>
	public class MshareDbContext : DbContext {
		/// <summary>
		/// User informations
		/// </summary>
		public DbSet<DaoUser> Users { get; set; }
		/// <summary>
		/// Group specific informations
		/// </summary>
		public DbSet<DaoGroup> Groups { get; set; }
		/// <summary>
		/// History informations
		/// </summary>
		public DbSet<DaoHistory> History { get; set; }
		/// <summary>
		/// Tokens that have been sent to users
		/// </summary>
		public DbSet<DaoEmailToken> EmailTokens { get; set; }
        /// <summary>
        /// Junction table for many-to-many user-group connections
        /// </summary>
        public DbSet<DaoUsersGroupsMap> UsersGroupsMap { get; set; }
        /// <summary>
        /// Spendings of the groups
        /// </summary>
        public DbSet<DaoSpending> Spendings { get; set; }
        /// <summary>
        /// Junction table with data for many-to-many debtor-spending connections
        /// </summary>
        public DbSet<DaoDebtor> Debtors { get; set; }

        public DbSet<DaoOptimizedDebt> OptimizedDebt { get; set;}

		public DbSet<DaoSettlement> Settlements { get; set; }

        /// <summary>
        /// Initializes a new DbContext
        /// </summary>
        /// <param name="options"></param>
        public MshareDbContext(DbContextOptions<MshareDbContext> options) :
            base(options) {

            this.ConfigureLogging(s => {
                System.Console.WriteLine(s);
            }, LoggingCategories.All);

        }

        /// <summary>
        /// Fluid rules
        /// </summary>
        /// <param name="modelBuilder"></param>
        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DaoUsersGroupsMap>()
                .HasKey(o => new { o.UserId, o.GroupId });

            modelBuilder.Entity<DaoEmailToken>()
                .HasKey(o => new { o.UserId, o.Token });

            modelBuilder.Entity<DaoDebtor>()
                .HasKey(o => new { o.DebtorUserId, o.SpendingId });

            modelBuilder.Entity<DaoOptimizedDebt>()
                .HasKey(o => new {o.GroupId, o.UserOwesId, o.UserOwedId });
        }
    }
}
