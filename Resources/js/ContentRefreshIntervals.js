(function(){
	var SECONDS_TO_MILLISECONDS_MULTIPLIER = 1000;

	var refreshInterval = null;

	function validate(timeout) {
		return timeout >= 1;
	}

	function refresh() {
		var timeout = parseInt(jq$('#refreshReportIntervals').find(":selected").val());
		clearInterval(refreshInterval);
		if (validate(timeout)) {
			timeout *= SECONDS_TO_MILLISECONDS_MULTIPLIER;
			refreshInterval = setInterval(function () {
				if (typeof (GetRenderedReportSet) == "function")
					GetRenderedReportSet();
				if (jq$('a[title="Refresh"]')) {
					jq$('a[title="Refresh"]').click();
				}
			}, timeout);
		}
	}

	jq$(document).ready(function () {
		jq$('#refreshReportIntervals').bind('change', refresh);
		refresh();
	});
})();