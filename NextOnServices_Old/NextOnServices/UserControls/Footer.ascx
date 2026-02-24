<%@ Control Language="C#" AutoEventWireup="true" CodeFile="Footer.ascx.cs" Inherits="UserControls_Footer" %>
<html>
<body>
    <footer class="footer container-fluid pl-30 pr-30">
        <div class="row">
            <div class="col-sm-12">
                <p>2017 &copy; All rights reserved. </p>
            </div>
        </div>
    </footer>

    <script src="../vendors/bower_components/jquery/dist/jquery.min.js"></script>
    <%--<script src="scripts/ui/jquery-1.4.2.js"></script>--%>
    <!-- Bootstrap Core JavaScript -->
    <script src="../vendors/bower_components/bootstrap/dist/js/bootstrap.min.js"></script>
    <!-- Counter Animation JavaScript -->
    <script src="../vendors/bower_components/waypoints/lib/jquery.waypoints.min.js"></script>
    <script src="../vendors/bower_components/jquery.counterup/jquery.counterup.min.js"></script>
    <!-- Data table JavaScript -->
    <%--<script src="vendors/bower_components/datatables/media/js/jquery.dataTables.min.js"></script>
	<script src="vendors/bower_components/datatables.net-buttons/js/dataTables.buttons.min.js"></script>
	<script src="vendors/bower_components/datatables.net-responsive/js/dataTables.responsive.min.js"></script>
	<script src="dist/js/responsive-datatable-data.js"></script>--%>

    <%--BlockUI--%>
    

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
    <script>
        $(document).ready(function () {



        });
        function changePassword() {

            $.ajax({
                type: "POST",
                contentType: "application/json; charset=utf-8",
                url: 'UsersDetails.aspx/ChangePassword',
                data: JSON.stringify({ opt: 0, oldpassword: $('#Header1_txtOldPwd').val(), newpassword: $('#Header1_txtNewPwd').val() }),
                cache: false,
                dataType: "json",
                success: function (data) {
                    if (data.d.length > 0) {
                        if (data.d == 1) {

                        }
                    }
                    else {
                        return false;
                        alert('There is some error');
                    }
                },
                error: function (result) {
                    alert(result.d);
                }
            });

        }
    </script>
</body>

</html>
