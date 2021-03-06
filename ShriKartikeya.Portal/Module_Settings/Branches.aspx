<%@ Page Language="C#" AutoEventWireup="true" MasterPageFile="~/MainMaster.master" CodeBehind="Branches.aspx.cs" Inherits="ShriKartikeya.Portal.Branches" %>

<asp:Content ID="RightOne" ContentPlaceHolderID="ContentPlaceHolder1" Runat="Server">

    <!-- CONTENT AREA BEGIN -->
    <div id="content-holder">
        <div class="content-holder">
            <div id="breadcrumb">
                <ul class="crumbs">
                    <li class="first"><a href="Settings.aspx" style="z-index: 9;"><span></span>Settings</a></li>
                    <li class="active"><a href="#" style="z-index: 7;" class="active_bread"> Branches</a></li>
                </ul>
            </div>
            <!-- DASHBOARD CONTENT BEGIN -->
            <div class="contentarea" id="contentarea">
                <div class="dashboard_center">
                    <div class="sidebox">
                        <div class="boxhead">
                            <h2 style="text-align: center">
                                Branches
                            </h2>
                        </div>
                        <div class="boxbody" style="padding: 5px 5px 5px 5px;">
                            <div class="boxin">
                                <div class="dashboard_firsthalf" style="width: 100%">
                                <table width="45%" cellpadding="5" cellspacing="5">
                        <tr>
                                        <td>
                                            <asp:Label ID="lblBranch" runat="server" Text="Branch :" class="fontstyle"></asp:Label>
                                        </td>
                                        <td>
                                            <asp:TextBox ID="Txt_Branch" runat="server" class="sinput"></asp:TextBox>
                                        </td>
                                        <td>
                                            <asp:Button ID="Btn_Branch" runat="server" Text="Add Branch" class="btn save" Width="120px"
                                                 OnClientClick='return confirm(" Are you sure you want to add the Branch?");' OnClick="Btn_Branch_Click" />
                                        </td>
                                    </tr>
                                </table>
                                </div>
                                    <div class="rounded_corners">
                                        <asp:GridView ID="gvBranch" runat="server" AutoGenerateColumns="false" Width="100%"
                                            
                                            PageSize="15" CellPadding="5" CellSpacing="3" ForeColor="#333333" GridLines="None" OnPageIndexChanging="gvBranch_PageIndexChanging" OnRowCancelingEdit="gvBranch_RowCancelingEdit" OnRowEditing="gvBranch_RowEditing" OnRowUpdating="gvBranch_RowUpdating">
                                            <RowStyle BackColor="#EFF3FB" />
                                            <RowStyle BackColor="#EFF3FB" Height="30" />
                                            <Columns>
                                                <asp:TemplateField HeaderText="S.No">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblSno" runat="server" Text="<%#Container.DataItemIndex+1 %>"></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblSno" runat="server" Text="<%#Container.DataItemIndex+1 %>"></asp:Label>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Branch">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBranchName" runat="server" Text="<%#Bind('BranchName') %>"></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:TextBox ID="txtBranchName" runat="server" Text="<%#Bind('BranchName') %>" Width="500px"></asp:TextBox>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Branch Id">
                                                    <ItemTemplate>
                                                        <asp:Label ID="lblBranchid" runat="server" Text="<%#Bind('Branchid') %>"></asp:Label>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:Label ID="lblBranchid" runat="server" Text="<%#Bind('Branchid') %>"></asp:Label>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                                <asp:TemplateField HeaderText="Operations">
                                                    <ItemTemplate>
                                                        <asp:LinkButton ID="linkedit" runat="server" CommandName="Edit" Text="Edit">
                                                        </asp:LinkButton>
                                                    </ItemTemplate>
                                                    <EditItemTemplate>
                                                        <asp:LinkButton ID="linkupdate" runat="server" CommandName="update" Text="Update"
                                                            OnClientClick='return confirm(" Are you  sure you  want to update  the Branch?");'  style="color:Black"></asp:LinkButton>
                                                        <asp:LinkButton ID="linkcancel" runat="server" CommandName="cancel" Text="Cancel"
                                                            OnClientClick='return confirm(" Are you  sure you  want to cancel  the Branch?");'  style="color:Black"></asp:LinkButton>
                                                    </EditItemTemplate>
                                                </asp:TemplateField>
                                            </Columns>
                                            <FooterStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" />
                                            <PagerStyle BackColor="#2461BF" HorizontalAlign="Center" BorderWidth="1px" CssClass="GridPager" />
                                            <SelectedRowStyle BackColor="#D1DDF1" Font-Bold="True" ForeColor="#333333" />
                                            <HeaderStyle BackColor="#507CD1" Font-Bold="True" ForeColor="White" Height="30" />
                                           <EditRowStyle ForeColor="#000" BackColor="#C2D69B" />
                                            <AlternatingRowStyle BackColor="White" />
                                        </asp:GridView>
                                    </div>
                                
                                
                            </div>
                    </div>
                </div>
            </div>
            <div class="clear">
            </div>
        </div>
    </div>
    <!-- DASHBOARD CONTENT END -->
    <!-- CONTENT AREA END -->
    </div>
 
</asp:Content>  