<%@ Page Title="" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TestPointScheduling.aspx.cs" Inherits="TT.ProjectOrbit.Monitoring.Pages.TestPointScheduling" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <script src="../Style%20Library/Monitoring/JS/jquery.dateformat.min.js"></script>
    <link href="../Style%20Library/Monitoring/DateTimePicker/jquery.datetimepicker.css" rel="stylesheet" />
    <script src="../Style%20Library/Monitoring/DateTimePicker/jquery.datetimepicker.js"></script>
    <script src="../Style%20Library/Monitoring/rrule/rrule.js"></script>
    <script src="../Style%20Library/Monitoring/rrule/nlp.js"></script>

    <script src="../Style%20Library/Monitoring/JS/Util.js"></script>
    <script src="../Style%20Library/Monitoring/JS/TestTypeHeader.js"></script>
    <script src="../Style%20Library/Monitoring/JS/TestPointScheduling.js"></script>
    <div class="monitoring-wrapper">
        <h2 class="monitoring-subheading">Test Point Location</h2>
        <div id="TestPointHeader"></div>

        <div class="monitoring-tabcontainer monitoring-margintop10">
            <div id="TestPointTabContainer">
            </div>
            <div id="TestPointCalibrationDocument_form" class="displayNone">
                <input type="hidden" name="SchedulerICal" />
                <input type="hidden" name="InstrumentID" />
            </div>
            <div id='TestPointScheduler_form' class="monitoring-tab">
                <table>
                    <tr>
                        <td valign="top">
                            <div>
                                <table id="AsBuiltVariables" data-name="variables" data-category="AsBuilt" data-prefix="V" class="monitoring-form monitoring-margintop10">
                                    <thead>
                                        <tr class="fixed">
                                            <th>Scheduling Method</th>
                                        </tr>
                                    </thead>
                                    <tbody>
                                        <tr>
                                            <td>
                                                <input type="radio" name="Method" /><label for="Method">Fixed period no phase</label></td>
                                            <td>
                                                <input type="radio" name="Method" /><label for="Method">Fixed period during phase</label></td>
                                            <td>
                                                <input type="radio" name="Method" /><label for="Method">Manual Selection</label></td>
                                        </tr>
                                    </tbody>
                                </table>
                            </div>
                        </td>
                    </tr>
                </table>

                <div class="monitoring-formbuttons">
                    <a href="javascript:void(0)" name="submit" class="monitoring-formbutton">Save</a> &nbsp;
                </div>
            </div>
        </div>
    </div>
    <table>
        <tbody>
            <tr>
                <td class="ms-formbody">
                    <div id="idCustomDIV">
                    </div>
                    <table cellspacing="0" cellpadding="0" border="0">
                        <tbody>
                            <tr>
                                <td class="monitoring-formlabelwidth">Date</td>
                                <td>
                                    <input id="eventDate" data-type="datetime" /></td>

                            </tr>
                            <tr>
                                <td>&nbsp;</td>
                            </tr>
                            <tr>
                                <td>Duration</td>
                                <td>
                                    <input class="monitoring-small" id="duration" type="text" value="1" />hour(s)</td>
                            </tr>
                            <tr id="RRule">
                                <td valign="top">
                                    <h3>Frequency</h3>
                                    <div>
                                        <input type="radio" data-val="RRule_DAILY" name="freq" checked="checked" />Daily
                                    </div>
                                    <div>
                                        <input type="radio" data-val="RRule_WEEKLY" name="freq" />Weekly
                                    </div>
                                    <div>
                                        <input type="radio" data-val="RRule_MONTHLY" name="freq" />Monthly
                                    </div>
                                    <div>
                                        <input type="radio" data-val="RRule_YEARLY" name="freq" />Yearly
                                    </div>
                                </td>
                                <td id="Pattern" valign="top">
                                    <h3>Pattern</h3>
                                    <div id="RRule_DAILY" class="patternDiv">
                                        <div>
                                            <input type="radio" data-val="first" name="pattern_DAILY" checked="checked" />Every
                                            <input type="text" class="monitoring-small" name="intervalDAILY" value="1" />days
                                        </div>
                                        <div>
                                            <input type="radio" data-val="second" name="pattern_DAILY" />Every weekday
                                        </div>
                                    </div>
                                    <div id="RRule_WEEKLY" class="patternDiv displayNone">
                                        <div>
                                            Recur Every
                                            <input type="text" value="1" name="intervalWEEKLY" class="monitoring-small" />Weeks(s) on :
                                        </div>
                                        <span>
                                            <input data-val="RRule.SU" name="pattern" type="checkbox" />Sunday</span>
                                        <span>
                                            <input data-val="RRule.MO" name="pattern" type="checkbox" checked />Monday</span>
                                        <span>
                                            <input data-val="RRule.TU" name="pattern" type="checkbox" />Tuesday</span>
                                        <span>
                                            <input data-val="RRule.WE" name="pattern" type="checkbox" />Wednesday</span>
                                        <span>
                                            <input data-val="RRule.TH" name="pattern" type="checkbox" />Thursday</span>
                                        <span>
                                            <input data-val="RRule.FR" name="pattern" type="checkbox" />Friday</span>
                                        <span>
                                            <input data-val="RRule.SA" name="pattern" type="checkbox" />Saturday</span>
                                    </div>
                                    <div id="RRule_MONTHLY" class="patternDiv displayNone">
                                        <div>
                                            <input name="pattern_MONTHLY" data-val="first" type="radio" checked="checked" />Day
                                            <input type="text" name="bymonthdayMONTHLY" value="1" class="monitoring-small" />of every
                                            <input type="text" name="intervalMONTHLY1" value="1" class="monitoring-small" />
                                            month(s)
                                        </div>
                                        <div>
                                            <input name="pattern_MONTHLY" data-val="second" type="radio" />The
                                            <select name="bysetposMONTHLY">
                                                <option value="1">First</option>
                                                <option value="2">Second</option>
                                                <option value="3">Third</option>
                                                <option value="4">Fourth</option>
                                                <option value="5">Fifth</option>
                                                <option value="111">Last</option>
                                            </select>
                                            <select name="byweekdayMONTHLY">
                                                <option value="RRule.SU">Sunday</option>
                                                <option value="RRule.MO">Monday</option>
                                                <option value="RRule.TU">Tuesday</option>
                                                <option value="RRule.WE">Wednesday</option>
                                                <option value="RRule.TH">Thursaday</option>
                                                <option value="RRule.FR">Friday</option>
                                                <option value="RRule.SA">Saturday</option>
                                            </select>
                                            of every
                                            <input type="text" name="intervalMONTHLY2" value="1" class="monitoring-small" />
                                            month(s)
                                        </div>
                                    </div>
                                    <div id="RRule_YEARLY" class="patternDiv displayNone">
                                        <div>
                                            <input name="pattern_YEARLY" data-val="first" type="radio" checked="checked" />Every
                                            <select name="bymonthYEARLY1">
                                                <option value="1">January</option>
                                                <option value="2">February</option>
                                                <option value="3">March</option>
                                                <option value="4">April</option>
                                                <option value="5">May</option>
                                                <option value="6">June</option>
                                                <option value="7">July</option>
                                                <option value="8">August</option>
                                                <option value="9">September</option>
                                                <option value="10">October</option>
                                                <option value="11">January</option>
                                                <option value="12">January</option>
                                            </select>
                                            <input type="text" value="1" name="bymonthdayYEARLY" class="monitoring-small" />
                                        </div>
                                        <div>
                                            <input name="pattern_YEARLY" data-val="second" type="radio" />The
                                            <select name="bysetposYEARLY">
                                                <option value="1">First</option>
                                                <option value="2">Second</option>
                                                <option value="3">Third</option>
                                                <option value="4">Fourth</option>
                                                <option value="5">Fifth</option>
                                                <option value="111">Last</option>
                                            </select>
                                            <select name="byweekdayYEARLY">
                                                <option value="RRule.SU">Sunday</option>
                                                <option value="RRule.MO">Monday</option>
                                                <option value="RRule.TU">Tuesday</option>
                                                <option value="RRule.WE">Wednesday</option>
                                                <option value="RRule.TH">Thursaday</option>
                                                <option value="RRule.FR">Friday</option>
                                                <option value="RRule.SA">Saturday</option>
                                            </select>
                                            of
                                            <select name="bymonthYEARLY2">
                                                <option value="1">January</option>
                                                <option value="2">February</option>
                                                <option value="3">March</option>
                                                <option value="4">April</option>
                                                <option value="5">May</option>
                                                <option value="6">June</option>
                                                <option value="7">July</option>
                                                <option value="8">August</option>
                                                <option value="9">September</option>
                                                <option value="10">October</option>
                                                <option value="11">January</option>
                                                <option value="12">January</option>
                                            </select>
                                        </div>
                                    </div>
                                </td>
                            </tr>
                            <tr>
                                <td colspan="2">
                                    <h3>End Logic</h3>
                                </td>
                            </tr>
                            <tr>
                                <%--<td valign="top">Start Date
                                    <input name="dtstart" data-type="date" />
                                </td>--%>
                                <td valign="top" colspan="2">
                                    <div>
                                        <input name="EndLogic" data-val="NoEndDate" type="radio" checked="checked" />No end date
                                    </div>
                                    <div>
                                        <input name="EndLogic" data-val="Occurences" type="radio" />End after
                                        <input type="text" class="monitoring-small" name="count" value="1" />
                                        Occurrence(s)
                                    </div>
                                    <div>
                                        <input name="EndLogic" data-val="EndDate" type="radio" />End by
                                        <input name="until" data-type="date" />
                                    </div>
                                </td>
                            </tr>
                        </tbody>
                    </table>
                </td>
            </tr>
        </tbody>
    </table>
    <div>
        <input type="button" onclick="generateICal()" value="Generate iCal"></input>
    </div>
    <script>
        var options;
        var rule;


        function generateICal() {

            var s = $("#eventDate").val();
            var d = s.split(" ")[0].split("/");
            var t = s.split(" ")[1].split(":");
            var eventStartDate = new Date(d[2] + "-" + d[1] + "-" + d[0]);
            eventStartDate.setHours(eventStartDate.getHours() + parseInt(t[0]));
            eventStartDate.setMinutes(eventStartDate.getMinutes() + parseInt(t[1]));

            var eventEndDate = new Date(d[2] + "-" + d[1] + "-" + d[0]);
            eventEndDate.setHours(eventEndDate.getHours() + parseInt(t[0]));
            eventEndDate.setMinutes(eventEndDate.getMinutes() + parseInt(t[1]));

            var duration = 1;
            try {
                duration = parseInt($("#duration").val());
            } catch (err) { }
            eventEndDate.setHours(eventEndDate.getHours() + duration);



            options = new Object();
            var interval = null;
            var byweekday = null;
            var bysetpos = null;
            var bymonth = null;
            var bymonthday = null;
            var until = null;
            var count = null;

            options.freq = eval($("[name=freq]:checked").attr("data-val").replace("_", "."));

            if (options.freq == RRule.DAILY) {
                if ($("[name=pattern_DAILY]:checked").attr("data-val") == "first") {
                    interval = $("[name=intervalDAILY]").val();
                }
                else {
                    byweekday = [RRule.MO, RRule.TU, RRule.WE, RRule.TH, RRule.FR];
                }
            }
            else if (options.freq == RRule.WEEKLY) {
                interval = $("[name=intervalWEEKLY]").val();
                byweekday = new Array();
                $("#RRule_WEEKLY").find("[name=pattern]:checked").each(function () {
                    byweekday.push(eval($(this).attr("data-val")));
                });
                if (byweekday.length == 0) byweekday.push(RRule.MO);
            }

            else if (options.freq == RRule.MONTHLY) {
                if ($("[name=pattern_MONTHLY]:checked").attr("data-val") == "first") {
                    bymonthday = new Array();
                    var s = $("[name=bymonthdayMONTHLY]").val();
                    if (!s) s = 1;
                    bymonthday.push(s);
                    interval = $("[name=intervalMONTHLY1]").val();
                } else {
                    byweekday = new Array();
                    bysetpos = $("[name=bysetposMONTHLY]").val();
                    interval = $("[name=intervalMONTHLY2]").val();
                    byweekday.push(eval($("[name=byweekdayMONTHLY]").val()));
                }
            }
            else if (options.freq == RRule.YEARLY) {
                if ($("[name=pattern_YEARLY]:checked").attr("data-val") == "first") {
                    bymonth = new Array();
                    bymonthday = new Array();
                    bymonth.push($("[name=bymonthYEARLY1]").val());
                    var s = $("[name=bymonthdayYEARLY]").val();
                    if (!s) s = 1;
                    bymonthday.push(s);
                } else {
                    byweekday = new Array();
                    bymonth = new Array();
                    bysetpos = $("[name=bysetposYEARLY]").val();
                    byweekday.push(eval($("[name=byweekdayYEARLY]").val()));
                    bymonth.push($("[name=bymonthYEARLY2]").val());
                }
            }

            if (interval) options.interval = interval;
            if (byweekday) options.byweekday = byweekday;
            if (bysetpos) options.bysetpos = bysetpos;
            if (bymonth) options.bymonth = bymonth;
            if (bymonthday) options.bymonthday = bymonthday;

            dtstart = $("[name=dtstart]").val();

            var endLogic = $("[name=EndLogic]:checked").attr("data-val");
            if (endLogic == "Occurences") {
                count = $("[name=count]").val();
            }
            if (endLogic == "EndDate") {
                until = $("[name=until]").val();
            }

            if (dtstart) {
                var k = dtstart.split("/");
                options.dtstart = new Date(k[2] + "-" + k[1] + "-" + k[0]);
            }
            if (until) {
                var k = until.split("/");
                options.until = new Date(k[2] + "-" + k[1] + "-" + k[0]);
            }

            options.dtstart = eventStartDate;

            if (count) options.count = count;
            rule = new RRule(options);
            console.log(rule.toString())


            var rr = rule.toString();
            if (rr.indexOf("BYSETPOS=111") > 0) rr = rr.replace("BYSETPOS=111", "BYSETPOS=-1")

            var iCal = "BEGIN:VCALENDAR\nPRODID:TTMONITORING\nVERSION:2.0";

            iCal += "\nBEGIN:VTIMEZONE";
            iCal += "\nTZID:Pacific/Auckland";
            iCal += "\nX-LIC-LOCATION:Pacific/Auckland";
            iCal += "\nBEGIN:DAYLIGHT";
            iCal += "\nTZOFFSETFROM:+1200";
            iCal += "\nTZOFFSETTO:+1300";
            iCal += "\nTZNAME:NZDT";
            iCal += "\nDTSTART:19700927T020000";
            iCal += "\nRRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=-1SU;BYMONTH=9";
            iCal += "\nEND:DAYLIGHT";
            iCal += "\nBEGIN:STANDARD";
            iCal += "\nTZOFFSETFROM:+1300";
            iCal += "\nTZOFFSETTO:+1200";
            iCal += "\nTZNAME:NZST";
            iCal += "\nDTSTART:19700405T030000";
            iCal += "\nRRULE:FREQ=YEARLY;INTERVAL=1;BYDAY=1SU;BYMONTH=4";
            iCal += "\nEND:STANDARD";
            iCal += "\nEND:VTIMEZONE";
            iCal += "\nBEGIN:VEVENT";
            iCal += "\nUID:" + guid();
            iCal += "\nDTSTAMP:" + $.format.date(new Date(), "yyyyMMddTHHmmss");
            iCal += "\nDTSTART:" + $.format.date(eventStartDate.toISOString(), "yyyyMMddTHHmmss");
            iCal += "\nDTEND:" + $.format.date(eventEndDate.toISOString(), "yyyyMMddTHHmmss");
            iCal += "\nSUMMARY:Z Day Party";
            iCal += "\nRRULE:" + rr;
            iCal += "\nEND:VEVENT\nEND:VCALENDAR";

            alert(iCal);

            

        }
        $(document).ready(function () {
            $("[name=freq]").change(function () {
                $(".patternDiv").addClass("displayNone");
                $("#" + $("[name=freq]:checked").attr("data-val")).removeClass("displayNone");
            });
            $("[data-type=date]").datepicker(
                {
                    dateFormat: "dd/mm/yy"
                });
            $("[data-type=datetime]").datetimepicker(
                {
                    format: "d/m/Y H:i"
                });
        });
        function guid() {
            function s4() {
                return Math.floor((1 + Math.random()) * 0x10000)
                  .toString(16)
                  .substring(1);
            }

            return s4() + s4() + '-' + s4() + '-' + s4() + '-' + s4() + '-' + s4() + s4() + s4();
        }

    </script>
</asp:Content>
