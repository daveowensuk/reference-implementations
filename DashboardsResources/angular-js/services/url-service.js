angular.module('izendaUrl').factory('$izendaUrl', ['$location', function ($location) {
	'use strict';

	return {
		urlSettings: new UrlSettings(),
		extractReportName: extractReportName,
		extractReportCategory: extractReportCategory,
		extractReportPartNames: extractReportPartNames,
		setReportFullName: setReportFullName,
		getReportInfo: getReportInfo
	};

	/**
	 * Get url settings for current report from url
	 */
	function getReportInfo() {
		return {
			fullName: getReportFullName(),
			name: getReportName(),
			category: getReportCategory(),
			isNew: getIsNew()
		};
	}

	/**
	 * Update report url: set parameter rn=reportFullName
	 */
	function setReportFullName(reportFullName) {
		$location.path(reportFullName.replace('\\', '/'));
	}

	/**
	 * Extract report name from category\report full name
	 */
	function extractReportName(fullName) {
		if (!angular.isString(fullName))
			return null;
		var reportFullNameParts = fullName.split('\\');
		if (reportFullNameParts.length == 2)
			return reportFullNameParts[1];
		else
			return reportFullNameParts[0];
	}

	/**
	 * Extract report category from category\report full name
	 */
	function extractReportCategory(fullName) {
		if (!angular.isString(fullName))
			return 'Uncategorized';
		var reportFullNameParts = fullName.split('\\');
		if (reportFullNameParts.length == 2)
			return reportFullNameParts[0];
		else
			return 'Uncategorized';
	}

	/**
	 * Extract report name, category, report set name for report part.
	 */
	function extractReportPartNames(reportFullName, isPartNameAtRight) {
		if (reportFullName == null)
			throw 'full name is null';
		var parseReportSetName = function (rsName) {
			if (rsName.indexOf('\\') > 0) {
				var p = rsName.split('\\');
				return {
					reportCategory: p[0],
					reportName: p[1]
				};
			}
			return {
				reportCategory: null,
				reportName: rsName
			};
		};

		var result = {
			reportPartName: null,
			reportFullName: reportFullName
		};
		var reportSetName = reportFullName;
		if (reportFullName.indexOf('@') >= 0) {
			var parts = reportFullName.split('@');
			if (!angular.isUndefined(isPartNameAtRight) && isPartNameAtRight) {
				result.reportPartName = parts[1];
				reportSetName = parts[0];
			} else {
				result.reportPartName = parts[0];
				reportSetName = parts[1];
			}
		}

		var reportNameObj = parseReportSetName(reportSetName);
		result.reportSetName = reportSetName;
		result.reportName = reportNameObj.reportName;
		result.reportCategory = reportNameObj.reportCategory;
		result.reportFullName = (result.reportPartName != null ? result.reportPartName + '@' : '') + result.reportSetName;
		return result;
	}

	function getIsNew() {
		if (angular.isUndefined($location.search()['isNew']))
			return null;
		return $location.search()['isNew'];
	}

	function getReportFullName() {
		if ($location.path().trim() === '')
			return null;
		var loc = $location.path();
		if ($location.path().charAt(0) == '/')
			loc = loc.substring(1);
		return loc.replace('/', '\\');
	}

	function getReportCategory() {
		return extractReportCategory(getReportFullName());
	}

	function getReportName() {
		return extractReportName(getReportFullName());
	}
}]);