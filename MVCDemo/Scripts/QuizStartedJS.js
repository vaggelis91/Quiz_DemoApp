$(document).ready(function () {
	$(".quizAnswers").hide();
	$(".quizContent").show();
	$("#showResultsBtn").click(function () {
		$(".quizAnswers").show();
	});
});