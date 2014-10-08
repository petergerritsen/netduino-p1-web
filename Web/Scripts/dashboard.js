function HourlyUsage(usages) {
    var self = this;

    self.firstUsages = ko.observableArray(usages.splice(0, (usages.length / 2) + (usages.length % 2)));
    self.secondUsages = ko.observableArray(usages);

    self.setUsages = function (usageLines) {
        self.firstUsages(usageLines.splice(0, (usageLines.length / 2) + (usageLines.length % 2)));
        self.secondUsages(usageLines);
    };
}

function HourlyUsageLine(hour, eTotal, pvProductionTotal, gas) {
    var self = this;

    self.hour = ko.observable(hour);
    self.eTotal = ko.observable(eTotal);
    self.pvProductionTotal = ko.observable(pvProductionTotal);
    self.gas = ko.observable(gas);
}

function Usage(identifierName, usages) {
    var self = this;
    self.identifierName = ko.observable(identifierName);
    self.usages = ko.observableArray(usages);
}

function UsageLine(identifier, eTotal, eReference, eDifference, ePercentage, pvProduction, gas, gasReference, gasDifference, gasPercentage) {
    var self = this;

    self.identifier = ko.observable(identifier);
    self.eTotal = ko.observable(eTotal);
    self.eReference = ko.observable(eReference);
    self.eDifference = ko.observable(eDifference);
    self.ePercentage = ko.observable(ePercentage);
    self.pvProduction = ko.observable(pvProduction);
    self.gas = ko.observable(gas);
    self.gasReference = ko.observable(gasReference);
    self.gasDifference = ko.observable(gasDifference);
    self.gasPercentage = ko.observable(gasPercentage);
}

function CurrentUsage(numberOfDays, e1meter, e2meter, pvProductionmeter, eTotal, eActualUsage, eReference, ePercentage, eRefYear, eEstimated, eRetourTotal, pvProduction, gasmeter, gas, gasRef, gasPercentage, gasRefYear, gasEstimated) {
    var self = this;

    self.numberOfDays = ko.observable(numberOfDays);
    self.e1meter = ko.observable(e1meter);
    self.e2meter = ko.observable(e2meter);
    self.pvProductionMeter = ko.observable(pvProductionmeter);
    self.eTotal = ko.observable(eTotal);
    self.eReference = ko.observable(eReference);
    self.ePercentage = ko.observable(ePercentage);
    self.eRefYear = ko.observable(eRefYear);
    self.eEstimated = ko.observable(eEstimated);
    self.eRetourTotal = ko.observable(eRetourTotal);
    self.pvProduction = ko.observable(pvProduction);
    self.eActualUsage = ko.observable(eActualUsage);
    self.pvProductionSelf = ko.computed(function () {
        return self.pvProduction() - self.eRetourTotal();
    });
    self.pvProductionSelfPerc = ko.computed(function () {
        return (self.pvProductionSelf() / self.pvProduction()) * 100;
    });
    self.gasMeter = ko.observable(gasmeter);
    self.gas = ko.observable(gas);
    self.gasRef = ko.observable(gasRef);
    self.gasPercentage = ko.observable(gasPercentage);
    self.gasRefYear = ko.observable(gasRefYear);
    self.gasEstimated = ko.observable(gasEstimated);
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

    self.hourlyUsage = ko.observable(new HourlyUsage([new HourlyUsageLine(0, 0, 0, 0), new HourlyUsageLine(0, 0, 0, 0)]));
    self.dailyUsage = ko.observable(new Usage('Day', [new UsageLine('', 0, 0, 0, 0, 0, 0, 0, 0, 0)]));
    self.weeklyUsage = ko.observable(new Usage('Week', [new UsageLine('', 0, 0, 0, 0, 0, 0, 0, 0, 0)]));
    self.monthlyUsage = ko.observable(new Usage('Month', [new UsageLine('', 0, 0, 0, 0, 0, 0, 0, 0, 0)]));
    self.currentUsage = ko.observable(new CurrentUsage(0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0));

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
    self.currentUrl = ko.computed(function () {
        return "/api/usages/" + self.apiKey() + "/estimated";
    });
    self.currentUsageUrl = ko.computed(function () {
        return "/api/usages/" + self.apiKey() + "/recent";
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
        var hourlyPvProd = [];
        var hourlyGas = [];
        var hourlyCats = [];
        $.each(data, function (index, value) {
            hourly.push(new HourlyUsageLine(value.Hour, value.ETotal, value.PvProduction, value.Gas));
            hourlyEle.push(value.ETotal);
            hourlyPvProd.push(value.PvProduction);
            hourlyGas.push(value.Gas);
            hourlyCats.push(value.Hour);
        });

        dashboardViewModel.hourlyUsage().setUsages(hourly);
        hourlyChart.series[0].setData(hourlyEle);
        hourlyChart.series[1].setData(hourlyPvProd);
        hourlyChart.series[2].setData(hourlyGas);
        hourlyChart.xAxis[0].setCategories(hourlyCats);

        $.mobile.loading('hide');
    });
}

function setChartData(usagedata, chart) {
    var ele = [];
    var eleRef = [];
    var pvProd = [];
    var gas = [];
    var gasRef = [];
    var cats = [];

    $.each(usagedata, function (index, value) {
        ele.push(value.eTotal());
        eleRef.push(value.eReference());
        pvProd.push(value.pvProduction());
        gas.push(value.gas());
        gasRef.push(value.gasReference());
        cats.push(value.identifier());
    });

    chart.series[0].setData(ele);
    chart.series[1].setData(gas);
    chart.series[2].setData(eleRef);
    chart.series[3].setData(gasRef);
    chart.series[4].setData(pvProd);
    chart.xAxis[0].setCategories(cats);
}

function loadDailyData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.dailyUrl(), function (data) {
        var daily = [];

        $.each(data, function (index, value) {
            daily.push(new UsageLine(value.DayString, value.ETotal, value.EleRef, value.EleRefDiff, value.EleRefPerc, value.PvProduction, value.Gas, value.GasRef, value.GasRefDiff, value.GasRefPerc));
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
            weekly.push(new UsageLine(value.Week, value.ETotal, value.EleRef, value.EleRefDiff, value.EleRefPerc, value.PvProduction, value.Gas, value.GasRef, value.GasRefDiff, value.GasRefPerc));
        });

        dashboardViewModel.weeklyUsage().usages(weekly);
        setChartData(weekly, weeklyChart);

        $.mobile.loading('hide');
    });
}

function loadMonthlyData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.monthlyUrl(), function (data) {
        var monthlyLines = [];
        var monthlyGraph = [];
        $.each(data, function (index, value) {
            var line = new UsageLine(value.MonthName, value.ETotal, value.EleRef, value.EleRefDiff, value.EleRefPerc, value.PvProduction, value.Gas, value.GasRef, value.GasRefDiff, value.GasRefPerc)
            monthlyLines.push(line);
            if (value.MonthName != 'Total') {
                monthlyGraph.push(line);
            }
        });

        dashboardViewModel.monthlyUsage().usages(monthlyLines);
        setChartData(monthlyGraph, monthlyChart);
        $.mobile.loading('hide');
    });
}

function loadCurrentData() {
    $.mobile.loading('show');
    $.getJSON(dashboardViewModel.currentUrl(), function (data) {
        dashboardViewModel.currentUsage(new CurrentUsage(data.NumberOfDays, data.E1Meter, data.E2Meter, data.PvProductionMeter, data.ETotal, data.EActualUsage, data.EleRef, data.EPercentage, data.ERefYear, data.EEstimated, data.ERetourTotal, data.PvProduction, data.GasMeter, data.Gas, data.GasRef, data.GasPercentage, data.GasRefYear, data.GasEstimated));
        $.mobile.loading('hide');
    });
    
    $.getJSON(dashboardViewModel.currentUsageUrl(), function (data) {
        var chartdata = [];
        var chartdataRetour = [];
        $.each(data, function (index, value) {
            chartdata.push({ x: moment(value.Timestamp).toDate().getTime(), y: value.CurrentUsage });
            chartdataRetour.push({ x: moment(value.Timestamp).toDate().getTime(), y: value.CurrentRetour });
        });

        currentChart.series[0].setData(chartdata);
        currentChart.series[1].setData(chartdataRetour);
        
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
	        '#AA4643',
            '#31B404'
        ],
        title: {
            text: title
        },
        yAxis: [{
            allowDecimals: false,
            labels: {
                formatter: function () {
                    return this.value + ' kWh';
                },
                style: {
                    color: '#4572A7'
                }
            },
            title: {
                text: 'Electricity',
                style: {
                    color: '#4572A7'
                }
            }
        },
        {
            allowDecimals: false,
            opposite: true,
            labels: {
                useHTML: true,
                formatter: function () {
                    return this.value + ' m<sup>3</sup>';
                },
                style: {
                    color: '#AA4643'
                }
            },
            title: {
                text: 'Gas',
                style: {
                    color: '#AA4643'
                }
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
        }, {
            yAxis: 0,
            type: 'column',
            name: 'Pv Production',
            data: [0]
        }]
    });

    return chart;
}

var dashboardViewModel = {};

var hourlyChart, dailyChart, weeklyChart, monthlyChart, currentChart;

$(document).ready(function () {
    $.mobile.loading('show');
    
    Highcharts.setOptions({ // This is for all plots, change Date axis to local timezone
        global: {
            useUTC: false
        }
    });

    hourlyChart = new Highcharts.Chart({
        chart: {
            renderTo: 'hourlychart',
            type: 'column'
        },
        colors: [
            '#4572A7',
	        '#31B404',
            '#AA4643'
        ],
        title: {
            text: 'Hourly usage'
        },
        yAxis: [{
            labels: {
                formatter: function () {
                    return this.value + ' kWh';
                },
                style: {
                    color: '#4572A7'
                }
            },
            title: {
                text: 'Electricity',
                style: {
                    color: '#4572A7'
                }
            }
        },
        {
            opposite: true,
            labels: {
                useHTML: true,
                formatter: function () {
                    return this.value + ' m<sup>3</sup>';
                },
                style: {
                    color: '#AA4643'
                }
            },
            title: {
                text: 'Gas',
                style: {
                    color: '#AA4643'
                }
            }
        }],
        series: [{
            yAxis: 0,
            name: 'Electricity',
            data: [0]
        }, {
            yAxis: 0,
            name: 'Production',
            data: [0]
        }, {
            yAxis: 1,
            name: 'Gas',
            data: [0]
        }]
    });
    
    currentChart = new Highcharts.Chart({
        chart: {
            renderTo: 'currentchart',
            type: 'spline',
            animation: Highcharts.svg, // don't animate in old IE
            marginRight: 10
        },
        colors: [
            '#4572A7',
	        '#31B404'
        ],
        title: {
            text: 'Current usage/retour'
        },
        xAxis: {
            type: 'datetime',
            tickPixelInterval: 100
        },
        yAxis: {
            min: 0,
            max: 10,
            title: {
                text: 'Value'
            },
            gridLineWidth: 0,
            plotBands: [{
                // Light usage
                from: 0,
                to: 2,
                color: 'rgba(68, 170, 20, 0.2)',
                label: {
                    text: 'Low',
                    style: {
                        color: '#606060'
                    }
                }
            },
                {
                    // Medium usage
                    from: 2.0,
                    to: 5.0,
                    color: 'rgba(255, 170, 50, 0.2)',
                    label: {
                        text: 'Medium',
                        style: {
                            color: '#606060'
                        }
                    }
                },
                {
                    // High usage
                    from: 5.0,
                    to: 7.5,
                    color: 'rgba(255, 0, 0, 0.2)',
                    label: {
                        text: 'High',
                        style: {
                            color: '#606060'
                        }
                    }
                },
                {
                    // Very high usage
                    from: 7.5,
                    to: 10,
                    color: 'rgba(255, 0, 0, 0.3)',
                    label: {
                        text: 'Very high',
                        style: {
                            color: '#606060'
                        }
                    }
                }
            ]
        },
        tooltip: {
            formatter: function () {
                return '<b>' + this.series.name + '</b><br/>' +
                Highcharts.dateFormat('%Y-%m-%d %H:%M:%S', this.x) + '<br/>' +
                Highcharts.numberFormat(this.y, 2);
            }
        },
        legend: {
            enabled: false
        },
        exporting: {
            enabled: false
        },
        series: [{
            name: 'Current usage',
            data: [0]
        }, {
            name: 'Current retour',
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
    loadCurrentData();

    ko.applyBindings(dashboardViewModel);
    
    var usageHubProxy = $.connection.usageHub;
    usageHubProxy.client.newCurrentUsage = function (timestamp, currentUsage, currentRetour) {
        currentChart.series[0].addPoint([moment(timestamp).toDate().getTime(), currentUsage], true, true);
        currentChart.series[1].addPoint([moment(timestamp).toDate().getTime(), currentRetour], true, true);
    };
    $.connection.hub.start().done(function() {
        usageHubProxy.server.joinHub(dashboardViewModel.apiKey());
    });
    
    $.mobile.loading('hide');
});