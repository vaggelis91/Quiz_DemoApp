$(document).ready(function () {
	$("#contactSection").hide();
	$(".statsForm").hide();

	$("#contact").click(function () {
		$("#contactSection").toggle();
	});
	$("#showStatsForm").click(function () {
		$(".statsForm").toggle();
	});
});