using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;
using Exam.Models;

namespace Exam.Models
{
    public class ViewModel
    {
        public User Name {get;set;}
        public int SessionData {get;set;}
        public User User {get;set;}
        public int UserId { get; set; }
        public int SessionId { get; set; }
        
        public int ActId { get; set; }
        public string ActName {get;set;}
        public string ActContent {get;set;}
        public User Creator {get;set;}
        public List<User> AllUsers {get;set;}
        public List<Act> AllActs {get;set;}
        public List<Going> AllGoings {get;set;}
        public bool IsMember()
        {
            bool canJoin = true;
            foreach (var go in AllGoings)
            {
                if(UserId == SessionData && ActId == this.ActId)
                {
                    canJoin = false;
                }
            }
            return canJoin;
        }

    }
}