/// <reference path="../../../scripts/jquery.d.ts" />
/// <reference path="../../../scripts/base.ts" />
/// <reference path="../../../lib/m-tools/m-tools.ts" />
namespace KotenBu.Backstage {
    /**
     * 权限页面
     */
    class PermissionsPage {
        public static PageData = {
            ID: null,
            FK_ParentID: null,
            Type: null
        }
        /**
         * 构造方法
         */
        constructor() {
            if (common.IsLogin(true)) {
                this.BindAllPermissionsType();
                this.BindEvent();
            }
        }
        /**
         * 绑定事件
         */
        private BindEvent() {
            MDMa.AddEvent("BtnAdd", "click", PermissionsPage.BtnAddEvent_Click);
            MDMa.AddEvent("BtnSave", "click", this.BtnSaveEvent_Click)
            MDMa.AddEvent("BtnDelete", "click", this.BtnDeleteEvent_Click)
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
            MDMa.AddEvent("InputIco", "invalid", function (e: Event) {
                let element = e.target as HTMLInputElement;
                let setting: InvalidOptionsModel = new InvalidOptionsModel();
                setting.Max = "长度不能超过" + element.maxLength;
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
         * 更改类型事件
         * @param e
         */
        private SearchPermissionsTypeEvent_Change(e: Event) {
            PermissionsPage.GetPermissionsInfoByType();
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
                PermissionsPage.GetPermissionsInfoByType();
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 根据类型获得权限信息
         */
        private static GetPermissionsInfoByType() {
            let url: string = "api/Permissions/GetPermissionsInfoByType";
            let data = {
                Type: MDMa.GetInputValue("SearchPermissionsType")
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let TreeList = MDMa.$("TreeList") as HTMLDivElement;
                TreeList.innerHTML = "";
                let treeElement = PermissionsPage.GetTreeElement(resM["Data"] as PermissionsModel[]);
                TreeList.appendChild(treeElement);
                PermissionsPage.BindTreeView();
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"])
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
            };
            common.SendGetAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 绑定树形结构
         * @param listM 数据源
         */
        private static GetTreeElement(listM: PermissionsModel[]): HTMLUListElement {
            let itemUl = document.createElement("ul");
            for (var i = 0; i < listM.length; i++) {
                let itemLi = document.createElement("li");
                let InfoSpan = document.createElement("span");
                itemLi.appendChild(InfoSpan);
                if (!MTMa.IsNullOrUndefinedOrEmpty(listM[i].Ico)) {
                    let icoI = document.createElement("i");
                    MDMa.AddClass(icoI, listM[i].Ico);
                    InfoSpan.appendChild(icoI);
                }
                let NameText = document.createTextNode(listM[i].Name);
                InfoSpan.appendChild(NameText);
                let btnGroup = document.createElement("div");
                itemLi.appendChild(btnGroup);
                MDMa.AddClass(btnGroup, "btn-group btn-group-xs TreeBtnGroup");
                if (i > 0) {
                    let BtnUp = document.createElement("button");
                    BtnUp.dataset.id = listM[i].ID;
                    BtnUp.dataset.targetid = listM[i - 1].ID;
                    MDMa.AddClass(BtnUp, "btn btn-primary glyphicon glyphicon-chevron-up");
                    MDMa.AddEvent(BtnUp, "click", PermissionsPage.BtnChangeRankEvent_Click);
                    btnGroup.appendChild(BtnUp);
                }
                if (i < listM.length - 1) {
                    let BtnDown = document.createElement("button");
                    BtnDown.dataset.id = listM[i].ID;
                    BtnDown.dataset.targetid = listM[i + 1].ID;
                    MDMa.AddClass(BtnDown, "btn btn-primary glyphicon glyphicon-chevron-down");
                    MDMa.AddEvent(BtnDown, "click", PermissionsPage.BtnChangeRankEvent_Click);
                    btnGroup.appendChild(BtnDown);
                }
                let BtnAdd = document.createElement("button");
                BtnAdd.dataset.id = listM[i].ID;
                BtnAdd.dataset.toggle = "modal";
                BtnAdd.dataset.target = "#EditModal";
                MDMa.AddClass(BtnAdd, "btn btn-success glyphicon glyphicon-plus");
                MDMa.AddEvent(BtnAdd, "click", PermissionsPage.BtnAddEvent_Click);
                btnGroup.appendChild(BtnAdd);
                let BtnEdit = document.createElement("button");
                BtnEdit.dataset.id = listM[i].ID;
                MDMa.AddClass(BtnEdit, "btn btn-warning glyphicon glyphicon-pencil");
                MDMa.AddEvent(BtnEdit, "click", PermissionsPage.BtnEditEvent_Click);
                btnGroup.appendChild(BtnEdit);
                let BtnRemove = document.createElement("button");
                BtnRemove.dataset.id = listM[i].ID;
                BtnRemove.dataset.toggle = "modal";
                BtnRemove.dataset.target = "#DeleteModal";
                MDMa.AddClass(BtnRemove, "btn btn-danger glyphicon glyphicon glyphicon-trash");
                MDMa.AddEvent(BtnRemove, "click", PermissionsPage.BtnRemoveEvent_Click);
                btnGroup.appendChild(BtnRemove);
                if (!MTMa.IsNullOrUndefined(listM[i].Items) && listM[i].Items.length > 0) {
                    let itemUl = PermissionsPage.GetTreeElement(listM[i].Items);
                    itemLi.appendChild(itemUl);
                }
                itemUl.appendChild(itemLi);
            }
            return itemUl;
        }
        /**
         * 绑定树形菜单
         */
        private static BindTreeView() {
            $('.tree li:has(ul)').addClass('parent_li').find(' > span').attr('title', 'Collapse this branch');
            $('.tree li.parent_li > span').on('click', function (e) {
                var children = $(this).parent('li.parent_li').find(' > ul > li');
                if (children.is(":visible")) {
                    children.hide('fast');
                    $(this).attr('title', 'Expand this branch').find(' > i').addClass('icon-plus-sign').removeClass('icon-minus-sign');
                } else {
                    children.show('fast');
                    $(this).attr('title', 'Collapse this branch').find(' > i').addClass('icon-minus-sign').removeClass('icon-plus-sign');
                }
                e.stopPropagation();
            });
        }
        /**
         * 新增按钮单击事件
         */
        private static BtnAddEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            common.ClearModalForm("EditModal");
            let EditModalLabel = MDMa.$("EditModalLabel") as HTMLHeadingElement;
            EditModalLabel.textContent = "新增";
            PermissionsPage.PageData.ID = null;
            PermissionsPage.PageData.FK_ParentID = MTMa.IsNullOrUndefined(btnElement.dataset.id) ? null : btnElement.dataset.id;
            PermissionsPage.PageData.Type = MDMa.GetInputValue("SearchPermissionsType");
        }
        /**
         * 编辑按钮单击事件
         */
        private static BtnEditEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            common.ClearModalForm("EditModal");
            let EditModalLabel = MDMa.$("EditModalLabel") as HTMLHeadingElement;
            EditModalLabel.textContent = "新增";
            PermissionsPage.PageData.ID = btnElement.dataset.id;
            PermissionsPage.PageData.FK_ParentID = null;
            PermissionsPage.PageData.Type = MDMa.GetInputValue("SearchPermissionsType");
            PermissionsPage.GetPermissionsInfoByID();
        }
        /**
         * 移除按钮单击事件
         */
        private static BtnRemoveEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            PermissionsPage.PageData.ID = btnElement.dataset.id;
        }
        /**
         * 调换位序
         * @param e
         */
        private static BtnChangeRankEvent_Click(e: MouseEvent) {
            let btnElement = e.target as HTMLButtonElement;
            btnElement.disabled = true;
            let url = "api/Permissions/ChangeRank";
            let data = {
                ID: btnElement.dataset.id,
                TargetID: btnElement.dataset.targetid
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                PermissionsPage.GetPermissionsInfoByType();
            };
            let FFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                common.ShowMessageBox(resM["Message"]);
            };
            let CFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                btnElement.disabled = false;
            };
            common.SendPostAjax(url, data, SFun, FFun, CFun);
        }
        /**
         * 根据ID获得权限信息
         * @param ID
         */
        private static GetPermissionsInfoByID() {
            let url = "api/Permissions/GetPermissionsInfoByID";
            let data = {
                ID: PermissionsPage.PageData.ID
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                let perM = resM["Data"];
                PermissionsPage.PageData.FK_ParentID = perM["FK_ParentID"];
                PermissionsPage.PageData.Type = perM["Type"];
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
            let data = PermissionsInputModel.GetInputData();
            if (data != null) {
                let BtnElement = e.target as HTMLButtonElement;
                BtnElement.textContent = "保存中......";
                BtnElement.disabled = true;
                let url: string;
                if (MTMa.IsNullOrUndefinedOrEmpty(PermissionsPage.PageData.ID)) {
                    url = "api/Permissions/AddPermissions";
                }
                else {
                    url = "api/Permissions/EditPermissions";
                }
                let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                    PermissionsPage.GetPermissionsInfoByType();
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
            let url = "api/Permissions/DeletePermissions";
            let data = {
                ID: PermissionsPage.PageData.ID
            };
            let SFun = function (resM: Object, xhr: XMLHttpRequest, state: number) {
                PermissionsPage.GetPermissionsInfoByType();
                $('#DeleteModal').modal('toggle');
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
    }
    /**
     * 权限模型
     */
    class PermissionsModel {
        /// <summary>
        /// 编号
        /// </summary>
        public ID: string;
        /// <summary>
        /// 名称
        /// </summary>
        public Name: string;
        /// <summary>
        /// 图标
        /// </summary>
        public Ico: string;
        /// <summary>
        /// 类型
        /// </summary>
        public Type: number;
        /// <summary>
        /// 子级
        /// </summary>
        public Items: PermissionsModel[];
    }
    /**
     * 权限输入模型
     */
    class PermissionsInputModel {
        /*唯一标识*/
        public ID: string;
        /*名称*/
        public Name: string;
        /*类型*/
        public Type: string;
        /*父级*/
        public FK_ParentID: string;
        /*权限代码*/
        public Code: string;
        /*图标*/
        public Ico: string;
        /*简介*/
        public Introduction: string;
        /*启用标识*/
        public IfEnable: boolean;
        /**
         * 获得输入模型
         */
        public static GetInputData(): PermissionsInputModel {
            let data: PermissionsInputModel = null;
            let InputForm = document.forms["InputForm"] as HTMLFormElement;
            if (!MTMa.IsNullOrUndefined(InputForm) && InputForm.checkValidity()) {
                data = {
                    ID: PermissionsPage.PageData.ID,
                    Name: MDMa.GetInputValue("InputName"),
                    Type: PermissionsPage.PageData.Type,
                    FK_ParentID: PermissionsPage.PageData.FK_ParentID,
                    Code: MDMa.GetInputValue("InputCode"),
                    Ico: MDMa.GetInputValue("InputIco"),
                    Introduction: MDMa.GetInputValue("InputIntroduction"),
                    IfEnable: (MDMa.$("InputIfEnable") as HTMLInputElement).checked,
                };
            }
            return data;
        }
    }
    /*页面加载完成时触发*/
    MDMa.AddEvent(window, "load", function () {
        let pageM = new PermissionsPage();
    });
}