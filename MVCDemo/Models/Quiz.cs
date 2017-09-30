using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MVCDemo.Models
{
	public class Quiz
	{
		public string Question { get; set; } 
		public string Answer1 { get; set; } 
		public string Answer2 { get; set; } 
		public string Answer3 { get; set; }
		public string Answer4 { get; set; }
		public string correctAnswers { get; set; } 
	}
}