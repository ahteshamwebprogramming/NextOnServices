<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Test.aspx.cs" Inherits="Test" %>



<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>

    <script type="text/javascript">

        function validation() {


            var txtName = document.getElementById('txtName');
            var txtSize = document.getElementById('txtSize');
            var txtCNumber = document.getElementById('txtCNumber');
            var txtCE = document.getElementById('txtCE');
            var ddlCountry = document.getElementById('ddlCountry');

            // alert('Asif');

            var result;



            if (txtName.value == '') {
                alertt('', 'Supplier name cannot left blank', 'error');
                document.getElementById('txtName').focus();
                return false;
            }
            if (txtSize.value == '') {
                alertt('', 'Sample Size cannot be left blank', 'error');
                document.getElementById('txtSize').focus();
                return false;
            }


            if (txtCNumber.value == '') {
                alertt('', 'Contact Number cannot left blank', 'error');
                document.getElementById('txtCNumber').focus();
                return false;
            }
            if (txtCE.value == '') {
                alertt('', 'Email cannot be left blank', 'error');
                document.getElementById('txtCE').focus();
                return false;
            }
            result = echeck(txtCE.value);
            if (result == false) {
                alertt('', ' Invalid Email-Id', 'error');
                document.getElementById('txtCE').focus();

                return false;
            }
            if (ddlCountry.value == '0') {
                alertt('', 'Please select country', 'error');
                document.getElementById('ddlCountry').focus();
                return false;
            }
            return true;
        }

        function confirmDelete() {
            return confirm('Are you sure you want to delete?');
        }
        function echeck(str) {
            var filter = /^([\w-]+(?:\.[\w-]+)*)@((?:[\w-]+\.)*\w[\w-]{0,66})\.([a-z]{2,6}(?:\.[a-z]{2})?)$/i
            if (filter.test(str))
                testresults = true;
            else {
                testresults = false;
            }
            return (testresults);
        }
        function alertt(message1, message2, message3) {
            swal(
                message1,
                message2,
                message3
            )
        }
        function success(message, typee) {
            swal({
                position: 'top-center',
                type: typee,
                title: message,
                showConfirmButton: false,
                timer: 2000
            })
        }
        function isNumberKey(evt) {
            var charCode = (evt.which) ? evt.which : event.keyCode; if (charCode != 46 && charCode != 45 && charCode > 31 && (charCode < 48 || charCode > 57)) { return false; }
            return true;
        };

    </script>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1"></div>
        </div>
        <!--/Preloader-->
        <div class="wrapper theme-1-active pimary-color-red">

            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>

            <div class="page-wrapper">
                <div class="container-fluid">

                    <!-- Title -->
                    <div class="row heading-bg">
                        <div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
                            <h5 class="txt-dark">Supplier</h5>
                        </div>

                        <!-- Breadcrumb -->
                        <div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
                            <ol class="breadcrumb">
                                <li><a href="Dashboard.aspx">Dashboard</a></li>
                                <%--<li><a href="#"><span>Supplier</span></a></li>--%>
                                <li class="active"><span>Add Supplier</span></li>
                            </ol>
                        </div>
                        <!-- /Breadcrumb -->

                    </div>
                    <!-- /Title -->

                    <!-- Row -->
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh" style="padding-bottom: 50px">
                                <div class="panel-heading">
                                    <div class="pull-left">
                                        <h6 class="panel-title txt-dark">Add Supplier:</h6>
                                    </div>
                                    <div class="clearfix"></div>
                                </div>
                                <div class="panel-body">
                                    <div class="row">
                                        <div class="col-md-4">
                                            <asp:TextBox runat="server" ID="txtIP" CssClass="form-control"></asp:TextBox>
                                        </div>
                                        <div class="col-md-4">
                                            <asp:Label CssClass="form-control" runat="server"></asp:Label>
                                        </div>

                                    </div>
                                    <div class="row" style="margin-top: 20px">
                                        <div class="col-md-4">
                                            <asp:Button runat="server" ID="btnCheckIP" Text="Check IP" OnClick="btnCheckIP_Click" />
                                        </div>
                                    </div>
                                    <div class="row">
                                        <div class="col-md-12">
                                            <input type="file" name="csvRecontacts" id="Recontactscsv" />
                                            <asp:Button runat="server" ID="uploadCSV" OnClick="uploadCSV_Click" Text="Upload" />
                                            <input type="button" value="upload by ajax" id="btn" onclick="uplaodData()" />
                                        </div>
                                    </div>
                                </div>





                            </div>
                        </div>

                    </div>
                    <div class="row">
                        <div class="col-xs-12">
                            <div class="panel panel-default card-view panel-refresh">
                                <div class="form-wrap">
                                    <div class="row">
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Success</label>
                                                <asp:TextBox ID="txtSuccess" runat="server" class="form-control" data-style="form-control btn-default btn-outline">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Default</label>
                                                <asp:TextBox ID="txtDefault" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Failure</label>
                                                <asp:TextBox ID="txtFailure" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Quality Termination</label>
                                                <asp:TextBox ID="ddlQuality" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                        <div class="col-md-12 col-sm-12 col-xs-12">
                                            <div class="form-group col-md-8">
                                                <label class="control-label mb-10">Over Quota</label>
                                                <asp:TextBox ID="ddlOverquota" runat="server" class="form-control" data-style="form-control btn-default btn-outline" TextMode="MultiLine" Rows="5">
                                                    
                                                </asp:TextBox>
                                            </div>
                                        </div>
                                    </div>
                                </div>
                                <br />
                            </div>
                        </div>

                    </div>

                </div>
                <!-- Footer -->
                <uc2:Footer ID="Footer1" runat="server" />
                <!-- /Footer -->

            </div>

        </div>

    </form>
    <script>
        function uplaodData() {

            var uploader;
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.csv)$/;

            //Checks whether the file is a valid csv file  
            if (regex.test($("#Recontactscsv").val().toLowerCase())) {

                if (typeof (FileReader) != "undefined") {


                    var reader = new FileReader();
                    reader.onload = function (e) {
                        // var rows = e.target.result.split(/\n(.+)/)[1];
                        var mystr = e.target.result;
                        var rows = mystr.substring(mystr.indexOf('\n') + 1);  //substring(e.target.result.substring.indexOf('\n') + 1);
                        $.ajax({
                            type: "POST",
                            contentType: "application/json; charset=utf-8",
                            url: 'test.aspx/test',
                            data: JSON.stringify({ fileupload: rows }),
                            //async: false,
                            cache: false,
                            dataType: "json",
                            success: function (data) {

                            },
                            error: function (result) {
                                return "error";
                            }
                        });
                    }



                    //reader.onload = function (e) {
                    //    var csvrows = e.target.result.split("\n");
                    //    var table = $("#Recontactscsv > tbody");
                    //    return table;
                    //}
                    reader.readAsText($("#Recontactscsv")[0].files[0]);

                    uploader = reader.result;

                    //return table;
                    //    }
                }

                var data = JSON.stringify(uploader);




            }
        }
        function ExportToTable() {
            var regex = /^([a-zA-Z0-9\s_\\.\-:])+(.csv)$/;
            //Checks whether the file is a valid csv file  
            if (regex.test($("#Recontactscsv").val().toLowerCase())) {
                if (typeof (FileReader) != "undefined") {
                    var reader = new FileReader();
                    //reader.onload = function (e) {
                    //    var csvrows = e.target.result.split("\n");
                    //    var table = $("#Recontactscsv > tbody");
                    //    return table;
                    //}
                    reader.readAsText($("#Recontactscsv")[0].files[0]);
                    var table = reader.result;
                    return table;
                }
            }
        }
    </script>
</body>
</html>
