<%@ Page Language="C#" AutoEventWireup="true" CodeFile="Default2.aspx.cs" Inherits="Default2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
    <form id="form1" runat="server">
        <div>
            <thead><tr>
                <th>Name</th>
                <th>Request Date</th>
                <th>Leave Type</th>
                <th>Leave Reason</th>
                <th>From Date</th>
                <th>To Date</th>
                <th>Leave Days</th>
                <th>View Details</th>
                <th>Approve</th>
                <th>Cancel</th>
            </tr>
            </thead>
            <tbody>
                <tr>
                    <td>Ruchika  Pant</td>
                    <td>07/02/2018</td>
                    <td>CL</td>
                    <td>cvxbxb</td>
                    <td>16/02/2018</td>
                    <td>16/02/2018</td>
                    <td>1</td>
                    <td><a href="#" onclick="onclickSearchWorkerLeavesReport(10055)">View Details</a></td>
                    <td><a href="#" onclick="ApproveLeave('10055','710067')">Approve</td>
                    <td><a href="#" onclick="CancelLeave('10055','710067')">Cancel</a></td>
                </tr>
                <tr>
                    <td>Ruchika  Pant</td>
                    <td>07/02/2018</td>
                    <td>CL</td>
                    <td>vnf</td>
                    <td>23/02/2018</td>
                    <td>23/02/2018</td>
                    <td>1</td>
                    <td><a href="#" onclick="onclickSearchWorkerLeavesReport(10056)">View Details</a></td>
                    <td><a href="#" onclick="ApproveLeave('10056','710067')">Approve</td>
                    <td><a href="#" onclick="CancelLeave('10056','710067')">Cancel</a></td>
                </tr>
                <tr>
                    <td>Ruchika  Pant</td>
                    <td>07/02/2018</td>
                    <td>LWP</td>
                    <td>dhbf</td>
                    <td>26/02/2018</td>
                    <td>26/02/2018</td>
                    <td>1</td>
                    <td><a href="#" onclick="onclickSearchWorkerLeavesReport(10057)">View Details</a></td>
                    <td><a href="#" onclick="ApproveLeave('10057','710067')">Approve</td>
                    <td><a href="#" onclick="CancelLeave('10057','710067')">Cancel</a></td>
                </tr>
                <tr>
                    <td>Ruchika  Pant</td>
                    <td>07/02/2018</td>
                    <td>CL</td>
                    <td>dagh</td>
                    <td>28/02/2018</td>
                    <td>28/02/2018</td>
                    <td>1</td>
                    <td><a href="#" onclick="onclickSearchWorkerLeavesReport(10058)">View Details</a></td>
                    <td><a href="#" onclick="ApproveLeave('10058','710067')">Approve</td>
                    <td><a href="#" onclick="CancelLeave('10058','710067')">Cancel</a></td>
                </tr>
            </tbody>
            
        </div>
    </form>
</body>
</html>
