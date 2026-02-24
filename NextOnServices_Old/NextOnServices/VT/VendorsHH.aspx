<%@ Page Language="C#" AutoEventWireup="true" CodeFile="VendorsHH.aspx.cs" Inherits="VT_VendorsHH" %>

<%@ Register Src="../UserControls/Header.ascx" TagName="Header" TagPrefix="uc1" %>
<%@ Register Src="../UserControls/Footer.ascx" TagName="Footer" TagPrefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
    <link href="https://cdn.datatables.net/1.10.16/css/jquery.dataTables.min.css" rel="stylesheet" />
    <link href="https://cdn.datatables.net/buttons/1.5.1/css/buttons.dataTables.min.css" rel="stylesheet" />
    <%--0000000000000000000000000000000000000000000000000000000000000--%>
    <style>
        table thead tr th {
            font-size: 11px !important;
            padding: 9px !important;
            font-weight: 500 !important;
        }

        table tbody tr td {
            font-size: 11px !important;
            padding: 6px !important;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="preloader-it">
            <div class="la-anim-1"></div>
        </div>
        <div class="wrapper theme-1-active pimary-color-red">
            <uc1:Header ID="Header1" runat="server" />
            <div class="right-sidebar-backdrop"></div>
            <div class="page-wrapper">
                <div class="container-fluid pt-25 ">
                    <div class="row">
                        <div class="col-sm-12">
                            <div class="panel panel-default card-view">
                                <div class="panel-heading">
                                </div>
                                <div class="panel-wrapper collapse in">
                                    <div class="panel-body">
                                        <div class="table-wrap">
                                            <table class="table" id="Suppliers">
                                                <thead>
                                                    <tr>
                                                        <th>Vendor</th>
                                                        <th>Vendor</th>
                                                        <th>Vendor</th>
                                                    </tr>
                                                </thead>
                                                <tbody>
                                                    <tr>
                                                        <td>q</td>
                                                        <td>s</td>
                                                        <td>rt</td>
                                                    </tr>
                                                    <tr>
                                                        <td>q</td>
                                                        <td>s</td>
                                                        <td>rt</td>
                                                    </tr>
                                                </tbody>
                                            </table>
                                        </div>
                                    </div>
                                </div>
                            </div>
                        </div>
                    </div>
                </div>
                <uc2:Footer ID="Footer1" runat="server" />
            </div>
        </div>

    </form>
    <script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <script src="//cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js" type="text/javascript"></script>
    <script src="https://cdn.datatables.net/1.10.16/js/jquery.dataTables.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/dataTables.buttons.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.flash.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jszip/3.1.3/jszip.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/pdfmake.min.js"></script>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/pdfmake/0.1.32/vfs_fonts.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.html5.min.js"></script>
    <script src="https://cdn.datatables.net/buttons/1.5.1/js/buttons.print.min.js"></script>
    <%--0000000000000000000000000000000--%>
    <%--<script src="../Scripts/UI/ui/minified/jquery-ui.min.js"></script>--%>
    <script src="https://cdnjs.cloudflare.com/ajax/libs/jquery.blockUI/2.70/jquery.blockUI.min.js"></script>

    <script src="../Scripts/Alerts.js"></script>
    <script src="../CustomJS/Common.js"></script>
    <script src="../CustomJS/VendorsHH.js"></script>

    <script type="text/javascript">
        var table;
        $(document).ready(function () {
            PageInitialization();

        });
    </script>
</body>
</html>
