/// <reference path="../../../scripts/jquery.d.ts" />
/// <reference path="../../../scripts/base.ts" />
/// <reference path="../../../lib/m-tools/m-tools.ts" />
namespace KotenBu.Backstage {
    /**
     * 用户组页面
     */
    class UserGroupPage {
        /*页面数据*/
        public static PageData = {
            ID: null
        }
        /**
         * 构造方法
         */
        constructor() {
            if (common.IsLogin(true)) {
                UserGroupPage.GetList();
                this.BindAllPermissionsType();
                this.BindEvent();
            }
        }
        /**
         * 绑定事件
         */
        private BindEvent() {
            MDMa.AddEvent("BtnSearch", "click", this.BtnSearchEvent_Click);
            MDMa.AddEvent("BtnAdd", "click", UserGroupPage.BtnAddEvent_Click);
            MDMa.AddEvent("BtnSave", "click", this.BtnSaveEvent_Click);
            MDMa.AddEvent("BtnDelete", "click", this.BtnDeleteEvent_Click);
            MDMa.AddEvent("BtnSavePermissions", "click", this.BtnSavePermissionsEvent_Click);
            MDMa.AddEvent("InputName", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                setting.Required = "不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputCode", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                setting.Required = "不能为空";
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("InputIntroduction", "invalid", function (e: Event) {
                let element = e.target as HTMLTextAreaElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
                common.InputInvalidEvent_Invalid(e, setting);
            });
            MDMa.AddEvent("SearchPermissionsType", "change", this.SearchPermissionsTypeEvent_Change);
        }
        /**
         * 获得列表
         */
        private static GetList() {
            let url: string = "api/UserGroup/GetUserGroupInfoByWhere";
            let data = UserGroupSearchModel.GetInputData();
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                UserGroupPage.BindListInfo(resM["Data"]);
                common.BindPageInfo(resM["PagingInfo"] as MPagingModel, UserGroupPage.GetList);
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 绑定列表信息
         * @param listM 
         */
        private static BindListInfo(listM: Array<any>): void {
            let DataTable = MDMa.$("DataTable") as HTMLTableSectionElement;
            DataTable.innerHTML = "";
            if (!MTMa.IsNullOrUndefined(listM)) {
                for (let i = 0; i < listM.length; i++) {
                    let Item = document.createElement("tr");
                    let Index = document.createElement("td");
                    Index.textContent = (common.PagingM.PageModel.PagingSize * (common.PagingM.PageModel.PagingIndex - 1) + i + 1).toString();
                    Item.appendChild(Index);
                    let Name = document.createElement("td");
                    Name.textContent = listM[i]["Name"];
                    Item.appendChild(Name);
                    let Code = document.createElement("td");
                    Code.textContent = listM[i]["Code"];
                    Item.appendChild(Code);
                    let Introduction = document.createElement("td");
                    Introduction.textContent = listM[i]["Introduction"];
                    Item.appendChild(Introduction);
                    let IfEnable = document.createElement("td");
                    IfEnable.textContent = listM[i]["IfEnable"] ? "启用" : "禁用";
                    Item.appendChild(IfEnable);
                    let Operation = document.createElement("td");
                    let OperationBtnGroup = document.createElement("div");
                    MDMa.AddClass(OperationBtnGroup, "btn-group");
                    let SetPermissionsBtn = document.createElement("button");
                    MDMa.AddClass(SetPermissionsBtn, "btn btn-primary");
                    SetPermissionsBtn.setAttribute("type", "button");
                    SetPermissionsBtn.textContent = "设置权限";
                    MDMa.AddEvent(SetPermissionsBtn, "click", UserGroupPage.BtnSetPermissionsEvent_Click);
                    SetPermissionsBtn.dataset.id = listM[i]["ID"];
                    OperationBtnGroup.appendChild(SetPermissionsBtn);
                    let EditBtn = document.createElement("button");
                    MDMa.AddClass(EditBtn, "btn btn-default");
                    EditBtn.setAttribute("type", "button");
                    EditBtn.textContent = "编辑";
                    MDMa.AddEvent(EditBtn, "click", UserGroupPage.BtnEditEvent_Click);
                    EditBtn.dataset.id = listM[i]["ID"];
                    OperationBtnGroup.appendChild(EditBtn);
                    let RemoveBtn = document.createElement("button");
                    MDMa.AddClass(RemoveBtn, "btn btn-danger");
                    RemoveBtn.setAttribute("type", "button");
                    RemoveBtn.textContent = "删除";
                    RemoveBtn.dataset.toggle = "modal";
                    RemoveBtn.dataset.target = "#DeleteModal";
                    MDMa.AddEvent(RemoveBtn, "click", UserGroupPage.BtnRemoveEvent_Click);
                    RemoveBtn.dataset.id = listM[i]["ID"];
                    OperationBtnGroup.appendChild(RemoveBtn);
                    Operation.appendChild(OperationBtnGroup);
                    Item.appendChild(Operation);
                    DataTable.appendChild(Item);
                }
            }
        }
        /**
         * 查询按钮
         * @param e
         */
        private BtnSearchEvent_Click(e: MouseEvent) {
            common.PagingM.PageModel.PagingIndex = 1;
            common.PagingM.PageModel.PagingCount = 99;
            UserGroupPage.GetList();
        }
        /**
         * 新增按钮单击事件
         */
        private static BtnAddEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            common.ClearModalForm("EditModal");
            let EditModalLabel = MDMa.$("EditModalLabel") as HTMLHeadingElement;
            EditModalLabel.textContent = "新增";
            UserGroupPage.PageData.ID = null;
        }
        /**
         * 编辑按钮单击事件
         */
        private static BtnEditEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            common.ClearModalForm("EditModal");
            let EditModalLabel = MDMa.$("EditModalLabel") as HTMLHeadingElement;
            EditModalLabel.textContent = "新增";
            UserGroupPage.PageData.ID = btnElement.dataset.id;
            UserGroupPage.GetUserGroupInfoByID();
        }
        /**
         * 移除按钮单击事件
         */
        private static BtnRemoveEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            UserGroupPage.PageData.ID = btnElement.dataset.id;
        }
        /**
         * 根据ID获得用户组信息
         * @param ID
         */
        private static GetUserGroupInfoByID() {
            let url = "api/UserGroup/GetUserGroupInfoByID";
            let data = {
                ID: UserGroupPage.PageData.ID
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let perM = resM["Data"];
                common.BindInputInfo(perM);
                $('#EditModal').modal('toggle');
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"]);
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 保存按钮单击事件
         * @param e
         */
        private BtnSaveEvent_Click(e: MouseEvent) {
            common.ClearErrorMessage();
            let data = UserGroupInputModel.GetInputData();
            if (data != null) {
                let BtnElement = e.target as HTMLButtonElement;
                BtnElement.textContent = "保存中......";
                BtnElement.disabled = true;
                let url: string;
                if (MTMa.IsNullOrUndefinedOrEmpty(UserGroupPage.PageData.ID)) {
                    url = "api/UserGroup/AddUserGroup";
                }
                else {
                    url = "api/UserGroup/EditUserGroup";
                }
                let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    UserGroupPage.GetList();
                    $('#EditModal').modal('toggle');
                };
                let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    common.ShowMessageBox(resM["Message"]);
                };
                let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    BtnElement.textContent = "保存";
                    BtnElement.disabled = false;
                };
                common.SendPostAjax(url, data, SFun, FFun, CFun);
            }
        }
        /**
         * 删除按钮单击事件
         * @param e
         */
        private BtnDeleteEvent_Click(e: MouseEvent) {
            let BtnElement = e.target as HTMLButtonElement;
            BtnElement.textContent = "删除中......";
            BtnElement.disabled = true;
            let url = "api/UserGroup/DeleteUserGroup";
            let data = {
                ID: UserGroupPage.PageData.ID
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                $('#DeleteModal').modal('toggle');
                UserGroupPage.GetList();
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"]);
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                BtnElement.textContent = "删除";
                BtnElement.disabled = false;
            };
            common.SendPostAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 获得所有权限类型枚举信息
         */
        private BindAllPermissionsType() {
            let url: string = "api/Permissions/GetAllPermissionsType";
            let data = null;
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let permissionsEnumMs = resM["Data"] as Array<any>;
                let SearchPermissionsType = MDMa.$("SearchPermissionsType") as HTMLSelectElement;
                SearchPermissionsType.innerHTML = "";
                for (var i = 0; i < permissionsEnumMs.length; i++) {
                    let option = new Option(permissionsEnumMs[i]["EnumName"], permissionsEnumMs[i]["EnumValue"]);
                    SearchPermissionsType.options.add(option);
                }
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 查询权限类型更改事件
         * @param e
         */
        private SearchPermissionsTypeEvent_Change(e: Event) {
            let SearchPermissionsType = e.target as HTMLSelectElement;
            let formGroups = document.getElementsByName("PermissionsTypeGroup");
            let formGroup = document.getElementById("PermissionsTypeGroup" + SearchPermissionsType.value);
            if (formGroup) {
                for (var i = 0; i < formGroups.length; i++) {
                    MDMa.AddClass(formGroups[i], "Close");
                }
                MDMa.RemoveClass(formGroup, "Close");
            }
        }
        /**
         * 设置权限信息按钮单击事件
         * @param e
         */
        private static BtnSetPermissionsEvent_Click(e: MouseEvent) {
            let BtnElement = e.target as HTMLButtonElement;
            BtnElement.disabled = true;
            let url = "api/Permissions/GetEnablePermissionsInfoByUserGroupID";
            let data = {
                UserGroupID: BtnElement.dataset.id
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                UserGroupPage.PageData.ID = BtnElement.dataset.id;
                let PermissionsList = MDMa.$("PermissionsList");
                PermissionsList.innerHTML = "";
                for (var i = 0; i < resM["Data"]["length"]; i++) {
                    let formGroup = UserGroupPage.GetPermissionsGroup(resM["Data"][i]);
                    PermissionsList.appendChild(formGroup);
                }
                $('#PermissionsModal').modal('toggle');
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"]);
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                BtnElement.disabled = false;
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 获得权限组
         * @param listM
         */
        public static GetPermissionsGroup(listM: Array<any>): HTMLDivElement {
            let SearchPermissionsType = MDMa.$("SearchPermissionsType") as HTMLSelectElement;
            let formGroup = document.createElement("div");
            MDMa.AddClass(formGroup, "form-group");
            formGroup.setAttribute("id", "PermissionsTypeGroup" + listM["Type"]);
            formGroup.setAttribute("name", "PermissionsTypeGroup");
            if (SearchPermissionsType.value != listM["Type"]) {
                MDMa.AddClass(formGroup, "Close");
            }
            let btnGroup = document.createElement("div");
            MDMa.AddClass(btnGroup, "btn-group");
            let BtnCheckAll = document.createElement("button");
            BtnCheckAll.setAttribute("type", "button");
            MDMa.AddClass(BtnCheckAll, "btn btn-default btn-xs");
            BtnCheckAll.textContent = "全选";
            MDMa.AddEvent(BtnCheckAll, "click", UserGroupPage.CheckAll);
            let BtnCheckInvert = document.createElement("button");
            BtnCheckInvert.setAttribute("type", "button");
            MDMa.AddClass(BtnCheckInvert, "btn btn-default btn-xs");
            BtnCheckInvert.textContent = "反选";
            MDMa.AddEvent(BtnCheckInvert, "click", UserGroupPage.CheckInvert);
            btnGroup.appendChild(BtnCheckAll);
            btnGroup.appendChild(BtnCheckInvert);
            formGroup.appendChild(btnGroup);
            let ul = UserGroupPage.GetPermissionsList(listM["Items"]);
            MDMa.AddClass(ul, "PermissionsList");
            formGroup.appendChild(ul);
            return formGroup;
        }
        /**
         * 获得权限列表
         */
        public static GetPermissionsList(listM: Array<Object>): HTMLUListElement {
            let ul = document.createElement("ul");
            for (let i = 0; i < listM["length"]; i++) {
                let li = document.createElement("li");
                let input = document.createElement("input");
                input.setAttribute("type", "checkbox");
                input.value = listM[i]["ID"];
                input.checked = listM[i]["IsHas"];
                li.appendChild(input);
                li.appendChild(document.createTextNode(listM[i]["Name"]));
                if (listM[i]["Items"] != null) {
                    let div = document.createElement("div");
                    MDMa.AddClass(div, "btn-group");
                    let BtnCheckAll = document.createElement("button");
                    MDMa.AddClass(BtnCheckAll, "btn btn-default btn-xs");
                    BtnCheckAll.setAttribute("type", "button");
                    BtnCheckAll.textContent = "全选";
                    MDMa.AddEvent(BtnCheckAll, "click", UserGroupPage.CheckAll);
                    let BtnCheckInvert = document.createElement("button");
                    BtnCheckInvert.setAttribute("type", "button");
                    MDMa.AddClass(BtnCheckInvert, "btn btn-default btn-xs");
                    BtnCheckInvert.textContent = "反选";
                    MDMa.AddEvent(BtnCheckInvert, "click", UserGroupPage.CheckInvert);
                    div.appendChild(BtnCheckAll);
                    div.appendChild(BtnCheckInvert);
                    let subUl = UserGroupPage.GetPermissionsList(listM[i]["Items"]);
                    li.appendChild(div);
                    li.appendChild(subUl);
                }
                ul.appendChild(li);
            }
            return ul;
        }
        /**
         * 全选
         * @param e
         */
        public static CheckAll(e: MouseEvent) {
            let BtnElement = e.target as HTMLButtonElement;
            let inputs = BtnElement.parentElement.parentElement.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++) {
                inputs[i].checked = true;
            }
        }
        /**
         * 反选
         * @param e
         */
        public static CheckInvert(e: MouseEvent) {
            let BtnElement = e.target as HTMLButtonElement;
            let inputs = BtnElement.parentElement.parentElement.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++) {
                inputs[i].checked = !inputs[i].checked;
            }
        }
        /**
         * 保存权限按钮单击事件
         * @param e
         */
        private BtnSavePermissionsEvent_Click(e: MouseEvent) {
            let BtnElement = e.target as HTMLButtonElement;
            BtnElement.textContent = "保存中......";
            BtnElement.disabled = true;
            let Permissions = [];
            let PermissionsList = MDMa.$("PermissionsList") as HTMLDivElement;
            let inputs = PermissionsList.getElementsByTagName("input");
            for (var i = 0; i < inputs.length; i++) {
                if (inputs[i].type == "checkbox" && inputs[i].checked) {
                    Permissions.push(inputs[i].value);
                }
            }
            let url = "api/UserGroup/SavePermissions";
            let data = {
                UserGroupID: UserGroupPage.PageData.ID,
                PermissionsIDs: Permissions
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                $('#PermissionsModal').modal('toggle');
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"]);
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                BtnElement.textContent = "保存";
                BtnElement.disabled = false;
            };
            common.SendPostAjax(url, data, SFun, FFun, CFun);
        }
    }
    /**
     * 用户组查询模型
     */
    class UserGroupSearchModel {
        /*名称*/
        public Name: string;
        /*用户组代码*/
        public Code: string;
        /*启用标识*/
        public IfEnable: boolean;
        /*当前页数*/
        public PageIndex: number;
        /*每页显示数量*/
        public PageSize: number;
        /**
         * 获得输入模型
         */
        public static GetInputData(): UserGroupSearchModel {
            let data: UserGroupSearchModel = {
                Name: MDMa.GetInputValue("SearchName"),
                Code: MDMa.GetInputValue("SearchCode"),
                IfEnable: MDMa.GetInputValue("SearchIfEnable"),
                PageIndex: common.PagingM.PageModel.PagingIndex,
                PageSize: common.PagingM.PageModel.PagingSize,
            };
            return data;
        }
    }
    /**
     * 用户组输入模型
     */
    class UserGroupInputModel {
        /*唯一标识*/
        public ID: string;
        /*名称*/
        public Name: string;
        /*用户组代码*/
        public Code: string;
        /*简介*/
        public Introduction: string;
        /*启用标识*/
        public IfEnable: boolean;
        /**
         * 获得输入模型
         */
        public static GetInputData(): UserGroupInputModel {
            let data: UserGroupInputModel = null;
            let InputForm = document.forms["InputForm"] as HTMLFormElement;
            if (!MTMa.IsNullOrUndefined(InputForm) && InputForm.checkValidity()) {
                data = {
                    ID: UserGroupPage.PageData.ID,
                    Name: MDMa.GetInputValue("InputName"),
                    Code: MDMa.GetInputValue("InputCode"),
                    Introduction: MDMa.GetInputValue("InputIntroduction"),
                    IfEnable: (MDMa.$("InputIfEnable") as HTMLInputElement).checked,
                };
            }
            return data;
        }
    }
    /*页面加载完成时触发*/
    MDMa.AddEvent(window, "load", function () {
        let pageM = new UserGroupPage();
    });
}