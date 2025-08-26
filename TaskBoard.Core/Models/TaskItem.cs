using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaskBoard.Core.Models
{
    /// <summary>A single to-do item inside TaskBoard.</summary>
    public sealed class TaskItem
    {
        public int Id { get; set; }                 // DB identity
        public string Title { get; set; } = "";     // required, 1..200
        public bool IsDone { get; set; }            // completion flag
        public DateTime CreatedUtc { get; set; }    // set server-side
    }
}
