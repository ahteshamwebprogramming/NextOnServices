<%@ Page Language="C#" AutoEventWireup="true" CodeFile="ProjectReport.aspx.cs" Inherits="ProjectReport" %>
<%@ Register src="../UserControls/Header.ascx" tagname="Header" tagprefix="uc1" %>
<%@ Register src="../UserControls/Footer.ascx" tagname="Footer" tagprefix="uc2" %>
<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head id="Head1" runat="server">
    <title></title>
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
            <div class="container-fluid">
            
            <!-- Title -->
					<div class="row heading-bg">
						<div class="col-lg-3 col-md-4 col-sm-4 col-xs-12">
							<h5 class="txt-dark">Project</h5>
						</div>
					
						<!-- Breadcrumb -->
						<div class="col-lg-9 col-md-8 col-sm-8 col-xs-12">
							<ol class="breadcrumb">
								<li><a href="Dashboard.aspx">Dashboard</a></li>
								<li><a href="#"><span>Project</span></a></li>
								<li class="active"><span>Project List</span></li>
							</ol>
						</div>
						<!-- /Breadcrumb -->
					
					</div>
					<!-- /Title -->
            
				<!-- Row -->
				<div class="row">
					<div class="col-sm-12">
						<div class="panel panel-default card-view">
							<div class="panel-heading">
								<div class="pull-left">
									<h6 class="panel-title txt-dark">Project Details</h6>
								</div>
								<div class="clearfix"></div>
							</div>
							<div class="panel-wrapper collapse in">
								<div class="panel-body">
									<div class="table-wrap">
										<div class="">
											<table id="myTable1" class="table table-hover display  pb-30" >
												<thead>
													<tr>
														<th>Project Name</th>														
														<th>Client Name</th>
														<th>Project Manager</th>
                                                       <%-- <th>Sample Size</th>--%>
                                                        <th>LOI</th>
														<th>Project Start Date</th>
                                                        <th>Project End Date</th>
                                                      <%--    <th>Country</th>--%>
														<th>Edit</th>
                                                       	<th>Url Mapping</th>
													</tr>
												</thead>
												
												<tbody> </tbody>									
												
												
											</table>
										</div>
									</div>
								</div>
							</div>
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
     <script src="https://cdn.datatables.net/1.10.11/js/jquery.dataTables.min.js" type="text/javascript"></script>
      <script type="text/javascript">




          //$('body').on('focus', ".datepicker", function () {
          //    $(this).datepicker();
          //});


          jQuery(document).ready(function () {



              $.ajax({
                  type: "POST",
                  contentType: "application/json; charset=utf-8",
                  url: 'ProjectDetails.aspx/GetProjectDetails',
                  cache: false,
                  dataType: "json",
                  success: function (data) {

                      var tbody = "";
                      for (var i = 0; i < data.d.length; i++) {

                          tbody += '<tr><td>' + data.d[i].PName + '</td><td>' + data.d[i].ClientName + '</td><td>' + data.d[i].PManager + '</td><td>' + data.d[i].LOI + '</td><td>' + data.d[i].SDate + '</td><td>' + data.d[i].EDate + '</td><td><input type="button" value="Edit" onclick=EDIT("' + data.d[i].ID + '") /></td><td><input type="button" value="Url Mapping" onclick=Mapping("' + data.d[i].ID + '","' + data.d[i].PName + '") /></td></tr>';
                          //$("#myTable1 tbody").append('<tr><td>' + data.d[i].Company + '</td><td>' + data.d[i].Person + '</td><td>' + data.d[i].Number + '</td><td>' + data.d[i].Email + '</td><td>' + data.d[i].Country + '</td></tr>')
                      }
                      $("#myTable1 tbody").html(tbody);
                      $("#myTable1").DataTable();
                  },
                  error: function (result) {
                      alert(result.d);
                  }
              });

              // $('#myTable1').DataTable();

          });


          function EDIT(id) {


              var url = "/UpdateProject.aspx?ID=" + id;
              window.location.href = url;



          }

          function Mapping(id, Name) {


              var url = "/ProjectUrls.aspx?ID=" + id + "&Name=" + Name;
              window.location.href = url;



          }
    </script>
</body>
</html>
