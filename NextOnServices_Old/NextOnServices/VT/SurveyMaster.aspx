<%@ Page Language="C#" AutoEventWireup="true" CodeFile="SurveyMaster.aspx.cs" Inherits="VT_SurveyMaster" %>

<%--<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>--%>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <meta charset="UTF-8" />
    <meta name="viewport" content="width=device-width, initial-scale=1.0, maximum-scale=1.0, user-scalable=no" />
    <title>index</title>
    <meta name="description" content="" />
    <meta name="keywords" content="" />
    <meta name="author" content="hencework" />

    <!-- Favicon -->
    <link rel="shortcut icon" href="favicon.ico" />
    <link rel="icon" href="../favicon.png" type="image/x-icon" />
    <link href="../vendors/bower_components/bootstrap/dist/css/bootstrap.min.css" rel="stylesheet" />
    <!-- Data table CSS -->
    <link href="../vendors/bower_components/datatables/media/css/jquery.dataTables.min.css" rel="stylesheet" type="text/css" />
    <link href="../vendors/bower_components/datatables.net-responsive/css/responsive.dataTables.min.css" rel="stylesheet" type="text/css" />
    <!-- select2 CSS -->
    <link href="../vendors/bower_components/select2/dist/css/select2.min.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-select CSS -->
    <link href="../vendors/bower_components/bootstrap-select/dist/css/bootstrap-select.min.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-tagsinput CSS -->
    <link href="../vendors/bower_components/bootstrap-tagsinput/dist/bootstrap-tagsinput.css" rel="stylesheet" type="text/css" />
    <!-- bootstrap-touchspin CSS -->
    <link href="../vendors/bower_components/bootstrap-touchspin/dist/jquery.bootstrap-touchspin.min.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Datetimepicker CSS -->
    <link href="../vendors/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <!-- multi-select CSS -->
    <link href="../vendors/bower_components/multiselect/css/multi-select.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Switches CSS -->
    <link href="../vendors/bower_components/bootstrap-switch/dist/css/bootstrap3/bootstrap-switch.min.css" rel="stylesheet" type="text/css" />
    <!-- Bootstrap Datetimepicker CSS -->
    <link href="../vendors/bower_components/eonasdan-bootstrap-datetimepicker/build/css/bootstrap-datetimepicker.min.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/UI/themes/base/jquery-ui.css" rel="stylesheet" />
    <link href="../Scripts/UI/themes/base/jquery.ui.theme.css" rel="stylesheet" />
    <link href="../dist/css/Alert.css" rel="stylesheet" />
    <%--  <link rel="stylesheet" href="http://ajax.googleapis.com/ajax/libs/jqueryui/1.8.5/themes/base/jquery-ui.css" type="text/css" media="all" />
    <link rel="stylesheet" href="http://static.jquery.com/ui/css/demo-docs-theme/ui.theme.css" type="text/css" media="all" />--%>
    <link href="../dist/css/CustmAhte.css" rel="stylesheet" />
    <!-- Custom CSS -->
    <link href="../dist/css/style.css" rel="stylesheet" type="text/css" />
    <link href="../Scripts/Dropdown/semantic.css" rel="stylesheet" />
    <script>
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
        function success(message, typee) {
            swal({ position: 'top-center', type: typee, title: message, showConfirmButton: false, timer: 2000 });
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete this?');
        }
    </script>
    <style type="text/css">
        .Gridview {
            font-family: Verdana;
            font-size: 10pt;
            font-weight: normal;
            color: black;
        }

        .container1_ppd input[type=checkbox] {
            position: unset;
            width: 30px;
        }

        .container1_ppd {
            float: left;
            margin-bottom: 10px;
        }

        .container2 {
            float: left;
            margin-bottom: 25px;
        }

        #lbl1 {
            font-size: 18px;
        }

        #QuestionWrap {
            font-size: 16px;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1">&nbsp;</div>
        </div>
        <!--/Preloader-->
        <div class="wrapper theme-1-active pimary-color-red">
            <div class="page-wrapper" style="margin-left: 0px">
                <div class="container">
                    <img id="PrescreeningLogo" src="../dist/img/logo.png" style="float: right" />
                    <!-- Title -->
                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <%--<h6 class="panel-title txt-dark">Questions List:</h6>--%>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="form-wrap" id="QuestionWrap">
                                    <%--       <div class="row">                                       
                                           <div class=" col-xs-12">
                                            <div class="form-group">--%>
                                    <div class="row" style="margin-bottom: 3%">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <asp:Label ID="lbl1" runat="server" class="control-label"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row" style="margin-left: 1%">
                                        <div class="col-xs-12">
                                            <div class="form-group">
                                                <div id="rdo1">
                                                </div>
                                                <div id="chk1">
                                                </div>
                                                <asp:TextBox ID="txt1" runat="server" TextMode="MultiLine" Rows="3" class="form-control"></asp:TextBox>
                                                <asp:Label ID="lblmesage" runat="server" class="control-label"></asp:Label>
                                            </div>
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-xs-12">
                                            <div class="form-group" style="text-align: center">
                                                <asp:Button ID="btnNext" runat="server" Text="Next"
                                                    class="btn btn-success btn-anim" OnClick="btnNext_Click" Visible="false" />
                                                <button type="button" class="btn btn-success btn-anim" id="btnprev" name="btnprev">Pervious</button>&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;
                                                <input type="button" class="btn btn-success btn-anim" id="btnSubmit" name="btnSubmit" value="Next" />
                                            </div>
                                        </div>
                                    </div>

                                    <%-- <div class="clearfix"></div>--%>
                                </div>
                            </div>
                            <br />
                        </div>
                    </div>
                </div>
            </div>
            <!-- Footer -->
            <%--   <uc2:Footer ID="Footer1" runat="server" />--%>
            <!-- /Footer -->
        </div>
        <asp:HiddenField ID="hdnid" runat="server" />
        <asp:HiddenField ID="hdnpid" runat="server" />
        <asp:HiddenField ID="hdnqtype" runat="server" />
        <asp:HiddenField ID="hdnURL" runat="server" />
        <asp:HiddenField ID="hdnOptions" runat="server" Value="0" />
    </form>
    <script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <!-- Bootstrap Core JavaScript -->
    <script src="../vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- Counter Animation JavaScript -->
    <script src="../vendors/bower_components/waypoints/lib/jquery.waypoints.min.js"></script>
    <script src="../vendors/bower_components/jquery.counterup/jquery.counterup.min.js"></script>
    <!-- Data table JavaScript -->
    <!-- Owl JavaScript -->
    <script src="../vendors/bower_components/owl.carousel/dist/owl.carousel.min.js"></script>
    <!-- Switchery JavaScript -->
    <script src="../vendors/bower_components/switchery/dist/switchery.min.js"></script>
    <!-- Slimscroll JavaScript -->
    <script src="../dist/js/jquery.slimscroll.js"></script>

    <!-- Fancy Dropdown JS -->
    <script src="../dist/js/dropdown-bootstrap-extended.js"></script>
    <%--<script src="https://unpkg.com/sweetalert2@7.0.7/dist/sweetalert2.all.js" type="text/javascript"></script>--%>
    <script src="../Scripts/Alerts.js"></script>

    <!-- Init JavaScript -->
    <script src="../dist/js/init.js"></script>
    <%--<script src="dist/js/dashboard3-data.js"></script>--%>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>
    <script type="text/javascript">
        jQuery(document).ready(function () {
            $('#QuestionWrap').hide();
            jQuery.blockUI({ message: '<h1 style="font-size: 25px;line-height:40px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
            //  alert('mohd');
            //  GetQuestionOptions1();
            var PreviousButton = 0, QuestionQID = 0, Logo = 0;
            $('#rdo1').hide();
            $('#chk1').hide();
            $('#txt1').hide();
            $('#lblmesage').hide();
            $('#btnprev').hide();
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'SurveyMaster.aspx/GetControls',
                data: JSON.stringify({}),
                cache: false,
                dataType: "json",
                success: function (data) {
                    if (data.d.length > 0) {
                        PreviousButton = data.d[0].PreviousButton;
                        QuestionQID = data.d[0].QuestionQID;
                        Logo = data.d[0].Logo;
                        if (Logo == 1) {
                            $('#PrescreeningLogo').show();
                        }
                        else {
                            $('#PrescreeningLogo').hide();
                        }
                    }
                },
                error: function () {
                    alert("Error While Getting Questions on Page Load");
                }
            });
            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'SurveyMaster.aspx/GetQuestions',
                data: JSON.stringify({ QID: 0, Action: 0, Cnt: 1 }),
                cache: false,
                dataType: "json",
                success: function (data) {


                    if (QuestionQID == 1)
                        $('#lbl1').text('(' + data.d[0].QID + ') ' + data.d[0].QLabel);
                    else
                        $('#lbl1').text(data.d[0].QLabel);
                    $('#hdnpid').val(data.d[0].PKID);
                    $('#hdnqtype').val(data.d[0].Qtype);
                    // $('#hdnqtype').val(2);
                    if ($('#hdnqtype').val() == 1) {
                        $('#rdo1').show();
                        $.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: 'SurveyMaster.aspx/GetOptions',
                            data: JSON.stringify({ QID: $('#hdnpid').val(), Action: 1, Cnt: 0 }),
                            cache: false,
                            dataType: "json",
                            success: function (data) {
                                if (data.d.length > 0) {

                                    for (var i = 0; i < data.d.length; i++) {
                                        var rdb = "<tr><td><label class='container2'><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><span class='checkmark1'></span></label><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                        $("#rdo1").append(rdb);
                                    }
                                }

                                jQuery.unblockUI();
                                $('#QuestionWrap').slideDown();
                            },
                            error: function () {
                                jQuery.unblockUI();
                                $('#QuestionWrap').slideDown();
                                alert("n");
                            }
                        });
                    }
                    else if ($('#hdnqtype').val() == 2) {
                        $('#chk1').show();
                        $.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: 'SurveyMaster.aspx/GetOptions',
                            data: JSON.stringify({ QID: $('#hdnpid').val(), Action: 1, Cnt: 0 }),
                            cache: false,
                            dataType: "json",
                            success: function (data) {
                                if (data.d.length > 0) {
                                    for (var i = 0; i < data.d.length; i++) {
                                        var rdb = "<tr><td> <label class='container1_ppd'><input id=rb" + data.d[i].optioncode + " type='checkbox' name='rbchk' value=" + data.d[i].optioncode + " /><span class='checkmark_ppd'></span></label><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                        $("#chk1").append(rdb);
                                    }
                                }
                                jQuery.unblockUI();
                                $('#QuestionWrap').slideDown();
                            },
                            error: function () {
                                jQuery.unblockUI();
                                $('#QuestionWrap').slideDown();
                                alert("n");
                            }
                        });
                    }
                    else {
                        $('#txt1').show();
                        jQuery.unblockUI();
                        $('#QuestionWrap').slideDown();
                    }

                },
                error: function () {
                    jQuery.unblockUI();
                    $('#QuestionWrap').slideDown();
                    alert("Error While Getting Questions on Page Load");
                }
            });
            $('#btnSubmit').click(function () {
                $('#QuestionWrap').hide();
                jQuery.blockUI({ message: '<h1 style="font-size: 25px;line-height:40px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
                var optval = "";
                if ($('#hdnqtype').val() == 1) {
                    optval = $('input[name=rbrdo]:checked').val();
                    //if (optval.length <= 0) {
                    //    alert("Please select at least option");
                    //    return;
                    //}
                }
                else if ($('#hdnqtype').val() == 2) {
                    $('[name="rbchk"]').each(function () {
                        if ($(this).prop('checked') == true) {
                            optval += $(this).val() + ',';
                            //if (optval.length <= 0) {
                            //    alert("Please select at least option");
                            //    return;
                            //}
                        }
                    });
                }
                else if ($('#hdnqtype').val() == 3) {
                    optval = $('#txt1').val();
                    //if (optval.length <= 0) {
                    //    alert("Please add comments");
                    //    return;
                    //}
                }
                if ((optval == '' || optval == 'undefined' || optval == undefined) && ($('#hdnOptions').val() == '1')) {
                    jQuery.unblockUI();
                    $('#QuestionWrap').slideDown();
                    alertt('', 'error', 'error');
                    return false;
                }
                if (optval == '' || optval == 'undefined' || optval == undefined) {
                    optval = 0;
                }
                $('#rdo1').hide();
                $('#chk1').hide();
                $('#txt1').hide();
                $('#lblmesage').hide();
                $('#btnprev').hide();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'SurveyMaster.aspx/NextProcess',
                    data: JSON.stringify({ QLabel: $('#lbl1').text(), QID: $('#hdnpid').val(), Options: optval, Action: 1 }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        if (data.d[0].QID < 0) {
                            if (data.d[0].QLabel == 'Terminate') {

                                if (data.d[0].QID == -1) {
                                    window.location = 'ErrorPage.aspx?Msg="Survey has been terminated"';
                                    return;
                                }
                                else if (data.d[0].QID == -2) {
                                    window.location = 'ErrorPage.aspx?Msg="Survey has been terminated as the quota for the selected answer is full"';
                                    return;
                                }
                            }
                            $('#rdo1').hide();
                            $('#chk1').hide();
                            $('#txt1').hide();
                            $('#lbl1').hide();
                            $('#btnprev').hide();
                            $('#btnSubmit').hide();
                            $('#lblmesage').show();
                            $('#lblmesage').text('Please Wait.......');
                            var url = $('#hdnURL').val();
                            window.location.href = url;
                            jQuery.unblockUI();
                            $('#QuestionWrap').slideDown();
                            return;
                        }
                        if (PreviousButton == 1)
                            $('#btnprev').show();
                        if (QuestionQID == 1)
                            $('#lbl1').text('(' + data.d[0].QID + ') ' + data.d[0].QLabel);
                        else
                            $('#lbl1').text(data.d[0].QLabel);
                        $('#hdnpid').val(data.d[0].PKID);
                        $('#hdnqtype').val(data.d[0].Qtype);
                        // $('#hdnqtype').val(2);
                        if ($('#hdnqtype').val() == 1) {
                            $('#rdo1').show();
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'SurveyMaster.aspx/GetOptions',
                                data: JSON.stringify({ QID: $('#hdnpid').val(), Action: 1, Cnt: 0 }),
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    var rdb = "";
                                    $("#rdo1").empty();
                                    if (data.d.length > 0) {
                                        $('#hdnOptions').val('1');
                                        for (var i = 0; i < data.d.length; i++) {
                                            // alert(data.d[i].OptionLabel);
                                            rdb = "<tr><td><label class='container2'><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><span class='checkmark1'></span></label><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                            $("#rdo1").append(rdb);
                                            //  $("#chkcreate").append("<input type='checkbox' name='Electricity' id='Electricity' value='true' checked = 'checked'><label for='Electricity'>Electricity</label>")
                                            // var option = $("<input type='radio' value='" + data.d[i].optioncode + "' />" + data.d[i].OptionLabel + "<br />");
                                            // $("#rdo1").append('<input type=radio  id=rb_ "' + data.d[i].optioncode + '"   value="' + data.d[i].optioncode + '">' + data.d[i].OptionLabel + "<br />");
                                            //  rdbtnlist.append(option);
                                        }
                                        $.ajax({
                                            type: "POST",
                                            contentType: "application/json; charset=utf-8",
                                            url: 'SurveyMaster.aspx/GetSelectedOptions',
                                            data: JSON.stringify({ QID: $('#hdnpid').val() }),
                                            cache: false,
                                            dataType: "json",
                                            success: function (data) {
                                                var rdb = "";
                                                //    $("#rdo1").empty();
                                                if (data.d.length > 0) {
                                                    for (var i = 0; i < data.d.length; i++) {
                                                        if (data.d[i].optioncode != '' && data.d[i].optioncode != null && data.d[i].optioncode != undefined)
                                                            $("input[name=rbrdo][value=" + data.d[i].optioncode + "]").prop('checked', true);
                                                        //  alert(data.d[i].optioncode);
                                                        //rdb = "<tr><td><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                                        //$("#rdo1").append(rdb);
                                                    }
                                                }
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                            },
                                            error: function (result) {
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                                alert(result.message);
                                            }
                                        });
                                    }
                                    else {
                                        $('#hdnOptions').val('0');
                                        jQuery.unblockUI();
                                        $('#QuestionWrap').slideDown();
                                    }
                                },
                                error: function () {
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                    alert("n");
                                }
                            });
                        }
                        else if ($('#hdnqtype').val() == 2) {
                            $('#chk1').show();
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'SurveyMaster.aspx/GetOptions',
                                data: JSON.stringify({ QID: $('#hdnpid').val(), Action: 1, Cnt: 0 }),
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    var rdb = "";
                                    $("#chk1").empty();
                                    if (data.d.length > 0) {
                                        $('#hdnOptions').val('1');
                                        for (var i = 0; i < data.d.length; i++) {
                                            rdb = "<tr><td><label class='container1_ppd'><input id=rb" + data.d[i].optioncode + " type='checkbox' name='rbchk' value=" + data.d[i].optioncode + " /><span class='checkmark_ppd'></span></label><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                            $("#chk1").append(rdb);
                                            //  $("<input type='radio' value='" + OptionText + "' />" + OptionText + "<br />");
                                            //  $("#chk1").append('< input type=radio  value="' + data.d[i].Id + '">' + data.d[i].SupplierName + "<br />");
                                        }
                                        // $('#ddlSuppliers').dropdown();


                                        $.ajax({
                                            type: "POST",
                                            contentType: "application/json; charset=utf-8",
                                            url: 'SurveyMaster.aspx/GetSelectedOptions',
                                            data: JSON.stringify({ QID: $('#hdnpid').val() }),
                                            cache: false,
                                            dataType: "json",
                                            success: function (data) {
                                                var rdb = "";
                                                //    $("#rdo1").empty();
                                                if (data.d.length > 0) {
                                                    for (var i = 0; i < data.d.length; i++) {
                                                        if (data.d[i].optioncode != '' && data.d[i].optioncode != null && data.d[i].optioncode != undefined)
                                                            $("input[name=rbchk][value=" + data.d[i].optioncode + "]").prop('checked', true);
                                                        //  alert(data.d[i].optioncode);
                                                        //rdb = "<tr><td><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                                        //$("#rdo1").append(rdb);
                                                    }
                                                }
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                            },
                                            error: function () {
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                                alert("n");
                                            }
                                        });
                                    }
                                    else {
                                        $('#hdnOptions').val('0');
                                        jQuery.unblockUI();
                                        $('#QuestionWrap').slideDown();
                                    }
                                },
                                error: function () {
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                    alert("n");
                                }
                            });
                        }
                        else {
                            $('#txt1').show();
                            $('#txt1').val('');
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'SurveyMaster.aspx/GetSelectedOptions',
                                data: JSON.stringify({ QID: $('#hdnpid').val() }),
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    var rdb = "";
                                    //    $("#rdo1").empty();
                                    if (data.d.length > 0) {
                                        $('#hdnOptions').val('1');
                                        for (var i = 0; i < data.d.length; i++) {
                                            if (data.d[i].optioncode != '' && data.d[i].optioncode != null && data.d[i].optioncode != undefined)
                                                $('#txt1').val(data.d[i].optioncode);
                                            // $("input[name=rbchk][value=" + data.d[i].optioncode + "]").prop('checked', true);
                                            //  alert(data.d[i].optioncode);
                                            //rdb = "<tr><td><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                            //$("#rdo1").append(rdb);
                                        }
                                    }
                                    else {
                                        $('#hdnOptions').val('0');
                                    }
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                },
                                error: function () {
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                    alert("n");
                                }
                            });
                        }
                    },
                    error: function (result) {
                        jQuery.unblockUI();
                        $('#QuestionWrap').slideDown();
                        alert("n");
                    }
                });
            });
            $('#btnprev').click(function () {
                $('#QuestionWrap').hide();
                jQuery.blockUI({ message: '<h1 style="font-size: 25px;line-height:40px;"><img src="../Imgs/busy.gif" style="height: 40px;margin-bottom: -10px;" /> Just a moment...</h1>' });
                $('#rdo1').hide();
                $('#chk1').hide();
                $('#txt1').hide();
                // $('#lbl1').hide();
                $.ajax({
                    type: "POST",
                    contentType: "application/json; charset=utf-8",
                    url: 'SurveyMaster.aspx/PreviusProcess',
                    // data: JSON.stringify({ QLabel: $('#lbl1').text(), QID: $('#hdnpid').val(), Options: optval, Action: 1 }),
                    cache: false,
                    dataType: "json",
                    success: function (data) {
                        if (data.d[0].QID == 0) {
                            $('#rdo1').hide();
                            $('#chk1').hide();
                            $('#txt1').hide();
                            $('#lbl1').hide();
                            $('#btnprev').hide();
                            $('#btnSubmit').hide();
                            $('#lblmesage').show();
                            $('#lblmesage').text('Survey is completed');
                            jQuery.unblockUI();
                            $('#QuestionWrap').slideDown();
                            return;
                        }
                        if (PreviousButton == 1)
                            $('#btnprev').show();
                        if (data.d[0].ID == 1) {
                            $('#btnprev').hide();
                        }
                        if (QuestionQID == 1)
                            $('#lbl1').text('(' + data.d[0].QID + ') ' + data.d[0].QLabel);
                        else
                            $('#lbl1').text(data.d[0].QLabel);
                        $('#hdnpid').val(data.d[0].PKID);
                        $('#hdnqtype').val(data.d[0].Qtype);
                        // $('#hdnqtype').val(2);
                        if ($('#hdnqtype').val() == 1) {
                            $('#rdo1').show();
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'SurveyMaster.aspx/GetOptions',
                                data: JSON.stringify({ QID: $('#hdnpid').val(), Action: 1, Cnt: 0 }),
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    var rdb = "";
                                    $("#rdo1").empty();
                                    if (data.d.length > 0) {
                                        for (var i = 0; i < data.d.length; i++) {
                                            rdb = "<tr><td><label class='container2'><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><span class='checkmark1'></span></label><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                            $("#rdo1").append(rdb);
                                        }
                                        $.ajax({
                                            type: "POST",
                                            contentType: "application/json; charset=utf-8",
                                            url: 'SurveyMaster.aspx/GetSelectedOptions',
                                            data: JSON.stringify({ QID: $('#hdnpid').val() }),
                                            cache: false,
                                            dataType: "json",
                                            success: function (data) {
                                                var rdb = "";
                                                //    $("#rdo1").empty();
                                                if (data.d.length > 0) {
                                                    for (var i = 0; i < data.d.length; i++) {
                                                        $("input[name=rbrdo][value=" + data.d[i].optioncode + "]").prop('checked', true);
                                                        //  alert(data.d[i].optioncode);
                                                        //rdb = "<tr><td><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                                        //$("#rdo1").append(rdb);
                                                    }
                                                }
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                            },
                                            error: function () {
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                                alert("n");
                                            }
                                        });
                                    }
                                    else {
                                        jQuery.unblockUI();
                                        $('#QuestionWrap').slideDown();
                                    }
                                },
                                error: function () {
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                    alert("n");
                                }
                            });
                        }
                        else if ($('#hdnqtype').val() == 2) {
                            $('#chk1').show();
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'SurveyMaster.aspx/GetOptions',
                                data: JSON.stringify({ QID: $('#hdnpid').val(), Action: 1, Cnt: 0 }),
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    var rdb = "";
                                    $("#chk1").empty();
                                    if (data.d.length > 0) {
                                        for (var i = 0; i < data.d.length; i++) {
                                            rdb = "<tr><td><label class='container1_ppd'><input id=rb" + data.d[i].optioncode + " type='checkbox' name='rbchk' value=" + data.d[i].optioncode + " /><span class='checkmark_ppd'></span></label><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                            $("#chk1").append(rdb);
                                            //  $("<input type='radio' value='" + OptionText + "' />" + OptionText + "<br />");
                                            //  $("#chk1").append('< input type=radio  value="' + data.d[i].Id + '">' + data.d[i].SupplierName + "<br />");
                                        }
                                        // $('#ddlSuppliers').dropdown();
                                        $.ajax({
                                            type: "POST",
                                            contentType: "application/json; charset=utf-8",
                                            url: 'SurveyMaster.aspx/GetSelectedOptions',
                                            data: JSON.stringify({ QID: $('#hdnpid').val() }),
                                            cache: false,
                                            dataType: "json",
                                            success: function (data) {
                                                var rdb = "";
                                                //    $("#rdo1").empty();
                                                if (data.d.length > 0) {
                                                    for (var i = 0; i < data.d.length; i++) {
                                                        $("input[name=rbchk][value=" + data.d[i].optioncode + "]").prop('checked', true);
                                                        //  alert(data.d[i].optioncode);
                                                        //rdb = "<tr><td><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                                        //$("#rdo1").append(rdb);
                                                    }
                                                }
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                            },
                                            error: function () {
                                                jQuery.unblockUI();
                                                $('#QuestionWrap').slideDown();
                                                alert("n");
                                            }
                                        });
                                    }
                                    else {
                                        jQuery.unblockUI();
                                        $('#QuestionWrap').slideDown();
                                    }
                                },
                                error: function () {
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                    alert("n");
                                }
                            });
                        }
                        else {
                            $('#txt1').show();
                            $.ajax({
                                type: "POST",
                                contentType: "application/json; charset=utf-8",
                                url: 'SurveyMaster.aspx/GetSelectedOptions',
                                data: JSON.stringify({ QID: $('#hdnpid').val() }),
                                cache: false,
                                dataType: "json",
                                success: function (data) {
                                    var rdb = "";
                                    //    $("#rdo1").empty();
                                    if (data.d.length > 0) {
                                        for (var i = 0; i < data.d.length; i++) {
                                            $('#txt1').val(data.d[i].optioncode);
                                            // $("input[name=rbchk][value=" + data.d[i].optioncode + "]").prop('checked', true);
                                            //  alert(data.d[i].optioncode);
                                            //rdb = "<tr><td><input id=rb" + data.d[i].optioncode + " type='radio' name='rbrdo' value=" + data.d[i].optioncode + " /><label for=lbl" + data.d[i].optioncode + ">" + data.d[i].OptionLabel + "</label></td></tr>";
                                            //$("#rdo1").append(rdb);
                                        }
                                    }
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                },
                                error: function () {
                                    jQuery.unblockUI();
                                    $('#QuestionWrap').slideDown();
                                    alert("n");
                                }
                            });
                        }
                    },
                    error: function () {
                        jQuery.unblockUI();
                        $('#QuestionWrap').slideDown();
                        alert("n");
                    }
                });
            });
        });
    </script>
</body>
</html>
