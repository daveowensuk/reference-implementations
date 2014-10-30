////////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Izenda Trace Logger
////////////////////////////////////////////////////////////////////////////////////////////////////////////////
var IzendaTrace = function () { };

IzendaTrace.prototype = {
	/**
	 * Ensure trace object is initialized
	 */
	ensureTrace: function () {
		this.trace = this.trace || {};
	},

	/**
	 * Set start date for trace
	 */
	startTrace: function (callee) {
		this.ensureTrace();
		this.trace[callee] = new Date();
	},

	/**
	 * Show trace message
	 */
	showTrace: function (callee, additionalString) {
		this.ensureTrace();
		var start = this.trace[callee];
		if (console && console.log)
			console.log((new Date().getTime() - start.getTime()) + 'ms: ' + callee + ' ' +
			  (additionalString == null ? '' : additionalString));
	}
};