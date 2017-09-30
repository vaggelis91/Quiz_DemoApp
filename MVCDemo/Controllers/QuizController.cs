using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Text;
using System.Web.Mvc;
using MVCDemo.Models;
using System.Data.SqlClient;


namespace MVCDemo.Controllers
{
    public class QuizController : Controller
    {
		string connString = @"data source = DESKTOP-07JF108\SQLEXPRESS; Initial Catalog = QuizDB; Integrated Security = SSPI";

		public ActionResult Home()
		{
			SelectionItems formOptionsList = new SelectionItems()
			{
				lessons = "History,Geography,Physics,Chemistry",
				numberOfQuestions = "10,20",
				categories = "High School,Senior High School,Higher Education"
			};
			return View(formOptionsList); 
		}

		[HttpPost]
		public ActionResult QuizStarted(SelectionItems selection)
		{
			return View(ExecuteQuery(selection.lessons, selection.numberOfQuestions, selection.categories));
		}

		public List<Quiz> ExecuteQuery(string lesson, string numberOfQuestions, string category)
		{
			List<int> limits = GetLimit(numberOfQuestions);
			List<Quiz> questionsList = new List<Quiz>();

			using (SqlConnection con = new SqlConnection(connString))
			{
				SqlCommand cmd = new SqlCommand("sp"+lesson, con);
				cmd.CommandType = System.Data.CommandType.StoredProcedure;

				SqlParameter categoryPar = new SqlParameter();
				categoryPar.ParameterName = "@category";
				categoryPar.Value = category;
				cmd.Parameters.Add(categoryPar);

				SqlParameter minLimitPar = new SqlParameter();
				minLimitPar.ParameterName = "@minLimit";
				minLimitPar.Value = limits[0];
				cmd.Parameters.Add(minLimitPar);

				SqlParameter maxLimitPar = new SqlParameter();
				maxLimitPar.ParameterName = "@maxLimit";
				maxLimitPar.Value = limits[1];
				cmd.Parameters.Add(maxLimitPar);

				con.Open();
				SqlDataReader dr = cmd.ExecuteReader();

				while (dr.Read())
				{
					questionsList.Add(new Quiz()
					{
						Question = dr.GetString(0),
						Answer1 = dr.GetString(1),
						Answer2 = dr.GetString(2),
						Answer3 = dr.GetString(3),
						Answer4 = dr.GetString(4),
						correctAnswers = dr.GetString(5)
					});
				}
			}	
			return questionsList;
		}

		public List<int> GetLimit(string numberOfQuestions)
		{
			List<int> limitList = new List<int>();

			Random rnd = new Random();
			//Generate a random number from 1 to 7 (7 is excluded). For example if the table e.g HistoryTB contains 500 rows, 
			//generate a number from 1 to 482, then add 9 or 19 (because of BETWEEN) and get this chunk of rows from HistoryTB.
			int lowerLimit = rnd.Next(1, 7);

			limitList.Add(lowerLimit);
			if (Convert.ToInt32(numberOfQuestions) == 10)
			{
				limitList.Add(lowerLimit + 9);
			}
			else
			{
				limitList.Add(lowerLimit + 19);
			}

			return limitList;
		}

		[HttpPost]
		public ActionResult FormSubmitted(string age, string education, string lessonsVariety, string qeustionsDifficulty, string usage, string rate)
		{		
			try
			{
				if (Convert.ToInt32(age) > 4 && Convert.ToInt32(age) < 101)
				{
					StringBuilder buildInsert = new StringBuilder();
					buildInsert.Append("'").Append(age).Append("','").Append(education).Append("','").Append(lessonsVariety).Append("','").Append(qeustionsDifficulty).Append("','").Append(usage).Append("','").Append(rate).Append("'");

					using (SqlConnection con = new SqlConnection(connString))
					{
						SqlCommand cmd = new SqlCommand("INSERT INTO StatsTB (Age, Education, Lessons, Difficulty, Usage, Rate) VALUES(" + buildInsert + ")", con);
						con.Open();
						SqlDataReader dr = cmd.ExecuteReader();
					}
					ViewBag.status = "Data submitted successfully. Thank you for your feedback!";
				}
				else
				{
					ViewBag.status = "Inavlid age. Please try again.";
				}
			}
			catch (Exception)
			{
				ViewBag.status = "Data submission failed. Please try again.";
			}

			return View();
		}

		public ActionResult ShowStats()
		{
			SqlConnection con = new SqlConnection(connString);

			string[] arr = new string[] { "spAges", "spRates", "spDifficulty" };
			foreach (string str in arr)
			{
				SqlCommand cmd = new SqlCommand(str, con);
				cmd.CommandType = System.Data.CommandType.StoredProcedure;
				con.Open();
				SqlDataReader dr = cmd.ExecuteReader();
				ExecuteProcedures(str, dr);
				con.Close();
			}

			return View();
		}

		private void ExecuteProcedures(string opt, SqlDataReader dr)
		{
			if (opt.Equals("spAges"))
			{
				while (dr.Read())
				{
					ViewBag.Ages = new List<int>()
					{
						dr.GetInt32(0),
						dr.GetInt32(1),
						dr.GetInt32(2)
					};
				}
			}
			else if (opt.Equals("spRates"))
			{
				while (dr.Read())
				{
					ViewBag.Rates = new List<int>()
					{
						dr.GetInt32(0),
						dr.GetInt32(1),
						dr.GetInt32(2)
					};
				}
			}
			else if (opt.Equals("spDifficulty"))
			{
				while (dr.Read())
				{
					ViewBag.Difficulty = new List<int>()
					{
						dr.GetInt32(0),
						dr.GetInt32(1),
						dr.GetInt32(2)
					};
				}
			}
		}

		public ActionResult CreateQuiz()
		{
			return View();
		}

		[HttpPost]
		public ActionResult NewQuiz(string[] userQuestion, string[] userAnswerA, string[] userAnswerB, string[] userAnswerC, string[] userAnswerD, string[] userCorrectAnswer)
		{
			List<Quiz> userQuiz = new List<Quiz>();

			for (int i = 0; i < userQuestion.Length; i++)
			{
				userQuiz.Add(new Quiz()
				{
					Question = userQuestion[i],
					Answer1 = userAnswerA[i],
					Answer2 = userAnswerB[i],
					Answer3 = userAnswerC[i],
					Answer4 = userAnswerD[i],
					correctAnswers = userCorrectAnswer[i]
				});
			}
			return View(userQuiz);

		}



	}
}