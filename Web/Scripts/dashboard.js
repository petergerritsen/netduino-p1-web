function HourlyUsage(usages) {
    var self = this;

    self.firstUsages = ko.observableArray(usages.splice(0, (usages.length / 2) + (usages.length % 2)));
    self.secondUsages = ko.observableArray(usages);

    self.setUsages = function (usageLines) {
        self.firstUsages(usageLines.splice(0, (usageLines.length / 2) + (usageLines.length % 2)));
        self.secondUsages(usageLines);
    };
}

function HourlyUsageLine(hour, eTotal, gas) {
    var self = this;

    self.hour = ko.observable(hour);
    self.eTotal = ko.observable(eTotal);
    self.gas = ko.observable(gas);
}

function Usage(identifierName, usages) {
    var self = this;
    self.identifierName = ko.observable(identifierName);
    self.usages = ko.observableArray(usages);
}

function UsageLine(identifier, eTotal, eReference, eDifference, ePercentage, gas, gasReference, gasDifference, gasPercentage) {
    var self = this;

    self.identifier = ko.observable(identifier);
    self.eTotal = ko.observable(eTotal);
    self.eReference = ko.observable(eReference);
    self.eDifference = ko.observable(eDifference);
    self.ePercentage = ko.observable(ePercentage);
    self.gas = ko.observable(gas);
    self.gasReference = ko.observable(gasReference);
    self.gasDifference = ko.observable(gasDifference);
    self.gasPercentage = ko.observable(gasPercentage);
}

function DashboardViewModel(apiKey, currentWeek) {
    var self = this;
    self.apiKey = ko.observable(apiKey);
    self.currentWeek = ko.observable(currentWeek);
    self.hourlyOffset = ko.observable(0);
    self.dailyOffset = ko.observable(0);
    self.weeklyOffset = ko.observable(0);
    self.weeklyStep = ko.observable(5);
    self.monthlyOffset = ko.observable(0);

    self.hourlyDate = ko.computed(function () {
        return moment().subtract('days', self.hourlyOffset()).format('ddd D MMM YYYY');
    });
    self.dailyWeek = ko.computed(function () {
        return self.currentWeek() - self.dailyOffset();
    });
    self.weeklyRange = ko.computed(function () {
        var endDate = moment().day(7).subtract('days', 7 * self.weeklyOffset());
        var startDate = moment(endDate).add('days', 1).subtract('days', 7 * self.weeklyStep());
        return startDate.format('ddd D MMM YYYY') + " - " + endDate.format('ddd D MMM YYYY');
    });
    self.monthlyYear = ko.computed(function () {
        var d = new Date();
        return d.getFullYear() - self.monthlyOffset();
    });

    self.hourlyUsage = ko.observable(new HourlyUsage([new HourlyUsageLine(0, 0, 0), new HourlyUsageLine(0, 0, 0)]));
    self.dailyUsage = ko.observable(new Usage('Day', [new UsageLine('', 0, 0, 0, 0, 0, 0, 0, 0)]));
    self.weeklyUsage = ko.observable(new Usage('Week', [new UsageLine('', 0, 0, 0, 0, 0, 0, 0, 0)]));
    self.monthlyUsage = ko.observable(new Usage('Month', [new UsageLine('', 0, 0, 0, 0, 0, 0, 0, 0)]));

    self.hourlyUrl = ko.computed(function () {
        return "/api/usages/" + self.apiKey() + "/hourly/" + self.hourlyOffset();
    });
    self.dailyUrl = ko.computed(function () {
        return "/api/usages/" + self.apiKey() + "/daily/" + self.dailyOffset();
    });
    self.weeklyUrl = ko.computed(function () {
        return "/api/usages/" + self.apiKey() + "/weekly/" + self.weeklyOffset() + "/" + self.weeklyStep();
    });
    self.monthlyUrl = ko.computed(function () {
        return "/api/usages/" + self.apiKey() + "/monthly/" + self.monthlyOffset();
    });

    self.hourlyNext = function () {
        if (this.hourlyOffset() == 0)
            return;

        this.hourlyOffset(this.hourlyOffset() - 1);
    };
    self.hourlyPrevious = function () {
        this.hourlyOffset(this.hourlyOffset() + 1);
    };
    self.dailyNext = function () {
        if (this.dailyOffset() == 0)
            return;

        this.dailyOffset(this.dailyOffset() - 1);
    };
    self.dailyPrevious = function () {
        this.dailyOffset(this.dailyOffset() + 1);
    };
    self.weeklyNext = function () {
        if (this.weeklyOffset() == 0)
            return;

        this.weeklyOffset(this.weeklyOffset() - 1);
    };
    self.weeklyPrevious = function () {
        this.weeklyOffset(this.weeklyOffset() + 1);
    };
    self.monthlyNext = function () {
        if (this.monthlyOffset() == 0)
            return;

        this.monthlyOffset(this.monthlyOffset() - 1);
    };
    self.monthlyPrevious = function () {
        this.monthlyOffset(this.monthlyOffset() + 1);
    };
}

function loadHourlyData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.hourlyUrl(), function (data) {
        var hourly = [];
        var hourlyEle = [];
        var hourlyGas = [];
        var hourlyCats = [];
        $.each(data, function (index, value) {
            hourly.push(new HourlyUsageLine(value.Hour, value.ETotal, value.Gas));
            hourlyEle.push(value.ETotal);
            hourlyGas.push(value.Gas);
            hourlyCats.push(value.Hour);
        });

        dashboardViewModel.hourlyUsage().setUsages(hourly);
        hourlyChart.series[0].setData(hourlyEle);
        hourlyChart.series[1].setData(hourlyGas);
        hourlyChart.xAxis[0].setCategories(hourlyCats);

        $.mobile.loading('hide');
    });
}

function setChartData(usagedata, chart) {
    var ele = [];
    var eleRef = [];
    var gas = [];
    var gasRef = [];
    var cats = [];

    $.each(usagedata, function (index, value) {
        ele.push(value.eTotal());
        eleRef.push(value.eReference());
        gas.push(value.gas());
        gasRef.push(value.gasReference());
        cats.push(value.identifier());
    });

    chart.series[0].setData(ele);
    chart.series[1].setData(gas);
    chart.series[2].setData(eleRef);
    chart.series[3].setData(gasRef);
    chart.xAxis[0].setCategories(cats);
}

function loadDailyData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.dailyUrl(), function (data) {
        var daily = [];

        $.each(data, function (index, value) {
            daily.push(new UsageLine(value.DayString, value.ETotal, value.EleRef, value.EleRefDiff, value.EleRefPerc, value.Gas, value.GasRef, value.GasRefDiff, value.GasRefPerc));
        });

        dashboardViewModel.dailyUsage().usages(daily);
        setChartData(daily, dailyChart);
        $.mobile.loading('hide');
    });
}

function loadWeeklyData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.weeklyUrl(), function (data) {
        var weekly = [];

        $.each(data, function (index, value) {
            weekly.push(new UsageLine(value.Week, value.ETotal, value.EleRef, value.EleRefDiff, value.EleRefPerc, value.Gas, value.GasRef, value.GasRefDiff, value.GasRefPerc));
        });

        dashboardViewModel.weeklyUsage().usages(weekly);
        setChartData(weekly, weeklyChart);

        $.mobile.loading('hide');
    });
}

function loadMonthlyData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.monthlyUrl(), function (data) {
        var monthly = [];
        $.each(data, function (index, value) {
            monthly.push(new UsageLine(value.MonthName, value.ETotal, value.EleRef, value.EleRefDiff, value.EleRefPerc, value.Gas, value.GasRef, value.GasRefDiff, value.GasRefPerc));
        });

        dashboardViewModel.monthlyUsage().usages(monthly);
        setChartData(monthly, monthlyChart);
        $.mobile.loading('hide');
    });
}

function createUsagePlusRefChart(container, title) {
    var chart = new Highcharts.Chart({
        chart: {
            renderTo: container
        },
        colors: [
            '#4572A7',
	        '#AA4643',
            '#4572A7',
	        '#AA4643'
        ],
        title: {
            text: title
        },
        yAxis: [{
            title: {
                text: 'Electricity (kWh)'
            }
        },
        {            
            opposite: true,
            title: {
                text: 'Gas (m3)'
            }
        }],
        plotOptions: {
            line: {
                dashStyle: 'Dash'
            }
        },
        series: [{
            yAxis: 0,
            type: 'column',
            name: 'Electricity',
            data: [0]
        }, {
            yAxis: 1,
            type: 'column',
            name: 'Gas',
            data: [0]
        }, {
            yAxis: 0,
            type: 'line',
            name: 'Electricity reference',
            data: [0]
        }, {
            yAxis: 1,
            type: 'line',
            name: 'Gas reference',
            data: [0]
        }]
    });

    return chart;
}

var dashboardViewModel = {};

var hourlyChart, dailyChart, weeklyChart, monthlyChart;

$(document).bind('pageinit', function () {
    $("#hourly").swipeleft(function () {
        dashboardViewModel.hourlyNext();
    });
    $("#hourly").swiperight(function () {
        dashboardViewModel.hourlyPrevious();
    });
    $("#daily").swipeleft(function () {
        dashboardViewModel.dailyNext();
    });
    $("#daily").swiperight(function () {
        dashboardViewModel.dailyPrevious();
    });
    $("#weekly").swipeleft(function () {
        dashboardViewModel.weeklyNext();
    });
    $("#weekly").swiperight(function () {
        dashboardViewModel.weeklyPrevious();
    });
    $("#monthly").swipeleft(function () {
        dashboardViewModel.monthlyNext();
    });
    $("#monthly").swiperight(function () {
        dashboardViewModel.monthlyPrevious();
    });
});

$(document).ready(function () {
    $.mobile.loading('show');
    hourlyChart = new Highcharts.Chart({
        chart: {
            renderTo: 'hourlychart',
            type: 'column'
        },
        title: {
            text: 'Hourly usage'
        },
        yAxis: [{
            title: {
                text: 'Electricity (kWh)'
            }
        },
        {               
            opposite: true,
            title: {
                text: 'Gas (m3)'
            }
        }],
        series: [{
            yAxis: 0,
            name: 'Elektricity',
            data: [0]
        }, {
            yAxis: 1,
            name: 'Gas',
            data: [0]
        }]
    });

    dailyChart = createUsagePlusRefChart('dailychart', 'Daily usage');
    weeklyChart = createUsagePlusRefChart('weeklychart', 'Weekly usage');
    monthlyChart = createUsagePlusRefChart('monthlychart', 'Monthly usage');

    dashboardViewModel = new DashboardViewModel($("input[name='apikey']").val(), $("input[name='currentWeek']").val());

    dashboardViewModel.hourlyUrl.subscribe(function (newValue) {
        loadHourlyData();
    });
    dashboardViewModel.dailyUrl.subscribe(function (newValue) {
        loadDailyData();
    });
    dashboardViewModel.weeklyUrl.subscribe(function (newValue) {
        loadWeeklyData();
    });
    dashboardViewModel.monthlyUrl.subscribe(function (newValue) {
        loadMonthlyData();
    });

    loadHourlyData();
    loadDailyData();
    loadWeeklyData();
    loadMonthlyData();

    ko.applyBindings(dashboardViewModel);
    $.mobile.loading('hide');
});