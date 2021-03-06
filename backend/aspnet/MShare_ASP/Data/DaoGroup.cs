﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MShare_ASP.Data
{
    /// <summary>Data Access Object for Group</summary>
    [Table("groups", Schema = "mshare")]
    public class DaoGroup
    {
        /// <summary>Primary key for Group</summary>
        [Key]
        [Column("id")]
        public long Id { get; set; }

        /// <summary>Name of the group</summary>
        [Column("name")]
        public string Name { get; set; }

        /// <summary>Id of the creator of this Group</summary>
        [Column("creator_user_id")]
        public long CreatorUserId { get; set; }

        /// <summary>The creator of this group</summary>
        [JsonIgnore]
        [ForeignKey("CreatorUserId")]
        public virtual DaoUser CreatorUser { get; set; }

        /// <summary>Weather this group has been deleted or not</summary>
        [Column("deleted", TypeName = "bit")]
        public bool Deleted { get; set; }

        /// <summary>All Users associted with this Group</summary>
        public IEnumerable<DaoUsersGroupsMap> Members { get; set; }

        /// <summary>All Spendings associated with this Group</summary>
        public IEnumerable<DaoSpending> Spendings { get; set; }
    }
}