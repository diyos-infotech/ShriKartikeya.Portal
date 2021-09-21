<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="signin.aspx.cs" Inherits="ShriKartikeya.Portal.signin" %>

<!DOCTYPE html>
<html lang="en">
<head>
    <meta charset="utf-8">
    <meta http-equiv="X-UA-Compatible" content="IE=edge">
    <meta name="viewport" content="width=device-width, initial-scale=1, shrink-to-fit=no">
    <title>signin</title>
    <meta name="title" content="Diyostech">
    <meta name="robots" content="all, index, follow">
    <meta name="author" content="Diyostech Infotech">
    <link rel="stylesheet" href="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/css/bootstrap.min.css">
    <link href="css/login.css" rel="stylesheet">
    <link rel="icon" href="images/favicon.png" type="image/x-icon">
</head>
<body>
    <div class="container">
        <div class="row mr-t-120">
            <div class="col-md-6">
                <img src="images/6909-1.svg" alt="" class="img-fluid">
            </div>
            <div class="col-md-6">
                <div class="login-card">
                    <div class="login-logo">
                        <img src="images/DIYOS_logo.png" alt="diyos infotech logo">
                    </div>
                    <form  runat="server">
                        <div class="form-group">
                            <input type="text" name="fullname" id="fullname" autocomplete="off" runat="server" placeholder="User Name" class="user-img">
                        </div>
                        <div class="form-group">
                            <input type="password" name="email" id="email" autocomplete="off" runat="server" placeholder="Password" class="paswd-img">
                        </div>
                        <div class="forgotpassword">
                            <%--<a href="#">Forgot password ?
                </a>--%>
                        </div>
                        <div class="form">
                             <asp:Button runat="server" ID="btn_Submit" class="btn btn-primary btn-lg btn-block active custm-loginbtn" Text="Sign In"
                     OnClick="btn_Submit_Click" />
                           <%-- <a class="btn btn-primary btn-lg btn-block active custm-loginbtn" role="button" id="btnsign" onserverclick="btn_Submit_Click"  runat="server" aria-pressed="true">Sign In</a>--%>
                        </div>
                        <!--<label class="form-check-label" for="check1">
                Remember me next time
                  <input type="checkbox" class="form-check-input" id="check1" name="option1" value="something" checked>
                </label>-->
                        <!-- Default checked -->
                        <div class="custom-control custom-switch">
                            <%--  <input type="checkbox" class="custom-control-input" id="customSwitch1" checked>
                            <label class="custom-control-label" for="customSwitch1">Toggle this switch element</label>--%>
                        </div>
                        <div class="login-footer-menu">
                            Copyright DIYOS INFOTECH PVT. LTD 2020
                            <br>
                            All Rights Reserved.
               
                            <a href="#">Privacy Policy</a>
                        </div>

                        <div>
                            <asp:Button ID="ButtonYes" Text="" Style="display: none" OnClick="ButtonYes_Click" runat="server" />
                            <asp:Button ID="ButtonNo" Text="" Style="display: none" OnClick="ButtonNo_Click" runat="server" />
                        </div>

                        <%-- jus a min --%>

                        <script type="text/javascript">

                            function show() {
                                debugger;
                                if (confirm("Do You Want To Logout all other session And Continue..?")) {
                                    document.getElementById("<%=ButtonYes.ClientID %>").click();

                                    return true;
                                }
                                else {
                                    document.getElementById("<%=ButtonNo.ClientID %>").click();
                                    return true;
                                }
                            }

                            function alert() {
                                alert("Invalid UserName/Password");
                            }

                        </script>


                        <asp:Label ID="lblerror" runat="server" Text="" Style="color: Red; margin-left: 210px;">  </asp:Label>
                        <asp:Label ID="lblcname" runat="server" Style="display: none"></asp:Label>
                    </form>
                </div>
            </div>
        </div>
    </div>
    <!--- Script ------------------->
    <script src="https://code.jquery.com/jquery-3.5.1.slim.min.js"></script>
    <script src="https://stackpath.bootstrapcdn.com/bootstrap/4.5.2/js/bootstrap.min.js"></script>
    <script>
        function ExecuteCodeBehindClickEvent() {
            //Get the Button reference and trigger the click event
            document.getElementById("btnSubmit").click();
        }
    </script>
</body>
</html>
