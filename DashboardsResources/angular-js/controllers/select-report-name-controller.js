izendaDashboardModule.controller('IzendaSelectReportNameController', ['$rootScope', '$scope', '$q', '$element', '$izendaUrl', '$izendaCommonQuery',
function IzendaSelectReportNameController($rootScope, $scope, $q, $element, $izendaUrl, $izendaCommonQuery) {
  'use strict';

  var _ = angular.element;

  $scope.isNewReportDialog = false;
  $scope.reportName = '';
  $scope.isCreatingNewCategory = false;
  $scope.newCategoryName = '';
  $scope.categories = [];
  $scope.selectedCategoryId = -1;
  $scope.reportSets = [];


  $scope.errorMessages = [];

  /**
   * Open modal event
   */
  $scope.$on('openSelectReportNameModalEvent', function (event, args) {
    $scope.isNewReportDialog = args.length > 0 ? args[0] : false;
    $scope.show();
  });

  /**
   * Set form to it's initial state
   */
  $scope.resetForm = function () {
    var reportInfo = $izendaUrl.getReportInfo();
    $scope.errorMessages.length = 0;
    $scope.isCreatingNewCategory = false;
    $scope.newCategoryName = '';
    $scope.selectedCategoryId = -1;
    $scope.categories.length = 0;
    $scope.reportSets.length = 0;
    if ($scope.isNewReportDialog) {
      $scope.reportName = '';
    } else {
      $scope.reportName = reportInfo.name;
    }
  };

  /**
   * Open modal dialog
   */
  $scope.show = function () {
    $scope.resetForm();

    var $modal = _($element);
    $modal.modal();

    $izendaCommonQuery.getReportSetCategory('Uncategorized').then(function (data) {
      $scope.categories.length = 0;
      $scope.reportSets = data.ReportSets;
      var reportInfo = $izendaUrl.getReportInfo();

      // add categories
      $scope.categories.push({
        'id': 0,
        'name': 'Create New'
      }, {
        'id': 1,
        'name': 'Uncategorized'
      });
      if (reportInfo.category == 'Uncategorized')
        $scope.selectedCategoryId = 1;
      for (var i = 0; i < $scope.reportSets.length; i++) {
        var id = $scope.categories.length;
        var report = $scope.reportSets[i];
        var category = report.Category;
        if (category == null || category == '')
          category = 'Uncategorized';
        if (_.grep($scope.categories, function (a) {
          return a['name'] === category;
        }).length == 0) {
          $scope.categories.push({
            'id': id,
            'name': category
          });
          if (reportInfo.category === category) {
            $scope.selectedCategoryId = id;
          }
        };
      }
    });
  };

  /**
   * Report category selected handler
   */
  $scope.categorySelectedHandler = function () {
    var selectedObj = $scope.getCategoryObjectById($scope.selectedCategoryId);
    if (selectedObj['id'] === 0) {
      $scope.isCreatingNewCategory = true;
    } else {
      $scope.isCreatingNewCategory = false;
    }
  };

  /**
   * OK button pressed
   */
  $scope.completeHandler = function () {
    $scope.validateForm().then(function () {
      var $modal = _($element);
      $modal.modal('hide');

      var selectedObj = $scope.getCategoryObjectById($scope.selectedCategoryId);
      var categoryName = $scope.isCreatingNewCategory ? $scope.newCategoryName : selectedObj['name'];
      $rootScope.$broadcast($scope.isNewReportDialog ? 'selectedNewReportNameEvent' : 'selectedReportNameEvent',
        [$scope.reportName, categoryName]);
    }, function () {
    });
  };

  /**
   * Validate form
   */
  $scope.validateForm = function () {
    return $q(function (resolve, reject) {
      $scope.errorMessages.length = 0;
      // check report name not empty
      var rName = $scope.reportName.trim();
      if (rName === '') {
        $scope.errorMessages.push('Report name can\'t be empty.');
        reject();
        return false;
      }

      // check category
      if ($scope.isCreatingNewCategory) {
        for (var i = 0; i < $scope.categories.length; i++) {
          if ($scope.newCategoryName == $scope.categories[i]['name']) {
            $scope.errorMessages.push('Category with name "' + $scope.newCategoryName + '" already exist.');
            reject();
            return false;
          }
        }
        resolve();
        return true;
      }

      // check report name
      var selectedObj = $scope.getCategoryObjectById($scope.selectedCategoryId);
      var selectedCategoryName = selectedObj['name'];

      // resolve if it is same report
      var reportInfo = $izendaUrl.getReportInfo();
      if (reportInfo.name === rName && reportInfo.category === selectedCategoryName) {
        resolve();
        return true;
      }

      // check report isn't in that category
      if (selectedCategoryName === 'Uncategorized') {
        if (isReportInReportList(rName, $scope.reportSets)) {
          $scope.errorMessages.push('Report "' + selectedCategoryName + '\\' + rName + '" already exist.');
          reject();
          return false;
        }
        resolve();
        return true;
      } else {
        $izendaCommonQuery.getReportSetCategory(selectedCategoryName).then(function (data) {
          $scope.reportSets = data.ReportSets;
          if (isReportInReportList(rName, data.ReportSets)) {
            $scope.errorMessages.push('Report "' + selectedCategoryName + '\\' + rName + '" already exist.');
            reject();
            return false;
          }
          resolve();
          return true;
        });
      }
      return true;
    });
  };

  /**
   * Get category object by it's id
   */
  $scope.getCategoryObjectById = function (id) {
    for (var i = 0; i < $scope.categories.length; i++) {
      if ($scope.categories[i].id === id)
        return $scope.categories[i];
    }
    return null;
  };

  /**
   * Get report with given name from report list
   */
  function isReportInReportList(reportName, reportList) {
    for (var i = 0; i < reportList.length; i++) {
      var rs = reportList[i];
      if (rs.Name === reportName.trim()) {
        return true;
      }
    }
    return false;
  }
}]);