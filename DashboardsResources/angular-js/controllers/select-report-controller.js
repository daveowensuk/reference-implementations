angular.module('izendaDashboard').controller('IzendaSelectReportController',
	['$rootScope', '$scope', '$q', '$izendaUrl', '$izendaCommonQuery',
	function IzendaSelectReportController($rootScope, $scope, $q, $izendaUrl, $izendaCommonQuery) {
		'use strict';

		$scope.$izendaUrl = $izendaUrl;
		$scope.category = 'Uncategorized';
		$scope.noReportsFound = false;
		$scope.tileId = null;
		$scope.categories = [];
		$scope.groups = [];

		/**
		 * Open modal event
		 */
		$scope.$on('openSelectPartModalEvent', function (event, args) {
			$scope.tileId = args.length > 0 ? args[0] : null;
			$scope.show();
		});

		/**
		 * Select report part modal
		 */
		$scope.show = function () {
			var $modal = angular.element('#selectPartModal');
			$modal.modal();
			$scope.categories.length = 0;
			setModalBodyLoading();
			$izendaCommonQuery.getReportSetCategory('Uncategorized').then(function (data) {
				var reportSets = data.ReportSets;
				addCategoriesToModal(reportSets);
				addReportsToModal(reportSets);
			});
		};

		/**
		 * Select category handler
		 */
		$scope.categoryChangedHandler = function () {
			setModalBodyLoading();
			$izendaCommonQuery.getReportSetCategory($scope.category).then(function (data) {
				addReportsToModal(data.ReportSets);
			});
		};

		/**
		 * User clicked to report set item
		 */
		$scope.itemSelectedHandler = function (item) {
			var isReportPart = item.isReportPart;
			var reportFullName = item.Name;
			if (item.Category != null && item.Category != '')
				reportFullName = item.Category + '\\' + reportFullName;

			if (!isReportPart) {
				// if report set selected
				setModalBodyLoading();
				$izendaCommonQuery.getReportParts(reportFullName).then(function(data) {
					var reports = data.Reports;
					addReportPartsToModal(reports);
				});
			} else {
				// if report part selected
				var $modal = angular.element('#selectPartModal');
				$modal.modal('hide');
				$rootScope.$broadcast('selectedReportPartEvent', [$scope.tileId, item]);
			}
		};

		/**
		 * Reset groups collection
		 */
		function setModalBodyLoading() {
			$scope.groups.length = 0;
			if (!$scope.$$phase)
				$scope.$apply();
		}

		/**
		 * Add reportset categories to modal select control.
		 */
		function addCategoriesToModal(reportSets) {
			if (reportSets == null)
				return;
			$scope.categories.length = 0;
			for (var i = 0; i < reportSets.length; i++) {
				var report = reportSets[i];
				if (report.Dashboard)
					continue;
				var category = report.Category;
				if (category == null || category == '')
					category = 'Uncategorized';
				if ($scope.categories.indexOf(category) < 0) {
					$scope.categories.push(category);
				}
			}
		}

		/**
		 * Add report to modal dialog body.
		 */
		function addReportsToModal(reportSets) {
			$scope.noReportsFound = false;
			var reportSetsToShow = angular.element.grep(reportSets, function (reportSet) {
				return !reportSet.Dashboard && reportSet.Name;
			});
			if (reportSetsToShow == null || reportSetsToShow.length == 0) {
				$scope.noReportsFound = true;
				return;
			}

			// add groups:
			var currentGroup = [];
			$scope.groups.length = 0;
			for (var i = 0; i < reportSetsToShow.length; i++) {
				if (i > 0 && i % 4 == 0) {
					$scope.groups.push(currentGroup);
					currentGroup = [];
				}
				var reportSet = reportSetsToShow[i];
				reportSet.isReportPart = false;
				currentGroup.push(reportSet);
			}
			$scope.groups.push(currentGroup);
		}

		/**
		 * Add report parts to modal
		 */
		function addReportPartsToModal(reportParts) {
			$scope.noReportsFound = false;
			if (reportParts == null || reportParts.length == 0) {
				$scope.noReportsFound = true;
				return;
			}

			// add groups:
			var currentGroup = [];
			$scope.groups.length = 0;
			for (var i = 0; i < reportParts.length; i++) {
				if (i > 0 && i % 4 == 0) {
					$scope.groups.push(currentGroup);
					currentGroup = [];
				}
				var reportPart = reportParts[i];
				reportPart.isReportPart = true;
				currentGroup.push(reportPart);
			}
			$scope.groups.push(currentGroup);
		}
	}
]);