izendaDashboardModule.controller('IzendaSelectReportController', ['$rootScope', '$scope', '$q', '$element', '$izendaUrl', '$izendaCommonQuery',
function IzendaSelectReportController($rootScope, $scope, $q, $element, $izendaUrl, $izendaCommonQuery) {
  'use strict';

  var _ = angular.element;

  $scope.izendaUrl = $izendaUrl;
  $scope.category = 'Uncategorized';
  $scope.isLoading = false;
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
    var $modal = _($element);
    $modal.modal();
    $scope.categories.length = 0;
    $scope.groups.length = 0;
    $scope.isLoading = true;
    $izendaCommonQuery.getReportSetCategory('Uncategorized').then(function (data) {
      var reportSets = data.ReportSets;
      addCategoriesToModal(reportSets);
      addReportsToModal(reportSets);
      $scope.isLoading = false;
      $scope.$evalAsync();
    });
  };

  /**
   * Select category handler
   */
  $scope.categoryChangedHandler = function () {
    $scope.isLoading = true;
    $scope.groups.length = 0;
    $izendaCommonQuery.getReportSetCategory($scope.category).then(function (data) {
      addReportsToModal(data.ReportSets);
      $scope.isLoading = false;
      $scope.$evalAsync();
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
      $scope.isLoading = true;
      $scope.groups.length = 0;
      $izendaCommonQuery.getReportParts(reportFullName).then(function (data) {
        var reports = data.Reports;
        addReportPartsToModal(reports);
        $scope.isLoading = false;
        $scope.$evalAsync();
      });
    } else {
      // if report part selected
      var $modal = _('#izendaSelectPartModal');
      $modal.modal('hide');
      $rootScope.$broadcast('selectedReportPartEvent', [$scope.tileId, item]);
    }
  };

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
    $scope.groups.length = 0;
    var reportSetsToShow = _.grep(reportSets, function (reportSet) {
      return !reportSet.Dashboard && reportSet.Name;
    });
    if (reportSetsToShow == null || reportSetsToShow.length == 0) {
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
    $scope.groups.length = 0;
    if (reportParts == null || reportParts.length == 0) {
      return;
    }

    // add groups:
    var currentGroup = [];
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