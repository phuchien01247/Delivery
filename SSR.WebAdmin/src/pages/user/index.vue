<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import {numeric, required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import Multiselect from "vue-multiselect";
import {userModel} from "@/models/userModel";

export default {
  page: {
    title: "Tài khoản",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout, Multiselect},
  data() {
    return {
      title: "Tài khoản",
      items: [
        {
          text: "Tài khoản",
          href: '/tai-khoan'
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      data: [],
      fields: [
        { key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: {width: '30px', minWidth: '30px'}
        },
        {
          key: "userName",
          label: "Tài khoản",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '80px', minWidth: '80px'},
        },
        {
          key: "fullName",
          label: "Họ và tên",
          class: 'td-ten',
          sortable: true,
        },
        {
          key: "role",
          label: "Quyền",
          class: 'td-ten',
          sortable: true,
          thStyle: {width: '80px', minWidth: '80px'},
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: {width: '130px', minWidth: '130px'},
        }
      ],
      currentPage: 1,
      perPage: 10,
      pageOptions: [5, 10, 25, 50, 100],
      showModal: false,
      showDeleteModal: false,
      submitted: false,
      sortBy: "age",
      filter: null,
      sortDesc: false,
      filterOn: [],
      numberOfElement: 1,
      totalRows: 1,
      model: userModel.baseJson(),
      listCoQuan: [],
      listRole: [],
      listChucVu: [],
      pagination: pagingModel.baseJson(),
      itemFilter:{
        code: null,
        name: null,
      }
    };
  },
  validations: {
    model: {
      userName: {required},
      firstName: {required},
      lastName: {required},
      role : {required},
      
    },
  },
  methods: {
    clearSearch(){
      this.itemFilter.code= null;
      this.itemFilter.name= null;
    },
    normalizer(node){
      if(node.children == null || node.children == 'null'){
        delete node.children;
      }
    },
    async fnGetList() {
         this.$refs.tblList?.refresh()
    },
    async getListRole(){
      await  this.$store.dispatch("roleStore/getAll").then((res) =>{
        this.listRole = res.data || [];
      })
    },
   

    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("userStore/delete", this.model.id).then((res) => {
          if (res.resultCode==='SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        });
      }
    },
    handleShowDeleteModal(id) {
      this.model.id = id;
      this.showDeleteModal = true;
    },
    async handleSubmit(e) {
      e.preventDefault();
      this.submitted = true;
      this.$v.$touch();
      if (this.$v.$invalid) {
        return;
      } else {
        let loader = this.$loading.show({
          container: this.$refs.formContainer,
        });
        if (
            this.model.id != 0 &&
            this.model.id != null &&
            this.model.id
        ) {
          // Update model
          await this.$store.dispatch("userStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.model= userModel.baseJson();
              this.$refs.tblList.refresh();
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("userStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList(); 
              this.model= userModel.baseJson();
              this.showModal = true;
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    async handleUpdate(id) {
      await this.$store.dispatch("userStore/getById", id).then((res) => {
        if (res.resultCode==='SUCCESS') {
          this.model = userModel.toJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    addCoQuanToModel : function (node, instanceId ){
      if(node.id){
        this.model.donVi = {id: node.id, ten: node.label};
      }
    },
    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("userStore/getPagingParams", params)
        return promise.then(resp => {
          let items = resp.data.data
          this.totalRows = resp.data.totalRows
          this.numberOfElement = resp.data.data.length
          this.loading = false
          return items || []
        })
      } finally {
        this.loading = false
      }
    },
    async handleExport() {
      await this.$store
          .dispatch("exportStore/dsUser")
          .then((res) => {
            console.log("Res", res)
            var blob = new Blob(
                [this.base64ToArrayBuffer(res.fileContents)],
                {type: "application/xlsx"}
            );
            var link = document.createElement("a");
            link.href = window.URL.createObjectURL(blob);
            var fileName = res.fileDownloadName;
            link.download = fileName;
            link.click();
          })
          .catch((e) => {
            console.log(e);
          });
    },
    base64ToArrayBuffer(base64) {
      var binaryString = window.atob(base64);
      var binaryLen = binaryString.length;
      var bytes = new Uint8Array(binaryLen);
      for (var i = 0; i < binaryLen; i++) {
        var ascii = binaryString.charCodeAt(i);
        bytes[i] = ascii;
      }
      return bytes;
    },
  },
  created(){
  
    this.getListRole();

  },
  mounted() {

  },
  watch: {
    model: {
      deep: true,
      handler(val) {
      }
    },
    showModal(status) {
      if (status == false) this.model = userModel.baseJson();
    },

    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    }
  },
};
</script>

<template>
  <Layout>
    <div class="row">
    <div class="col-12">
      <div class="card mb-2">
        <div class="card-body">
          <div class="row">
            <div class="col-md-4 col-12 d-flex align-items-center">
              <h4 class="font-size-18 fw-bold cs-title-page">Tài khoản</h4>
            </div>
            <div class="col-md-8 col-12 text-end">
              <b-button
                  type="button"
                  variant="primary"
                  class="w-md"
                  size="sm"
                  @click="showModal = true"
              >
                <i class="mdi mdi-plus me-1"></i> Tạo tài khoản
              </b-button>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <div class="col-sm-4">
                <div class="search-box me-2 mb-2 d-inline-block">
                  <div class="position-relative">
                    <input
                        v-model = "filter"
                        type="text"
                        class="form-control"
                        placeholder="Tìm kiếm ..."
                    />
                    <i class="bx bx-search-alt search-icon"></i>
                  </div>
                </div>
              </div>
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal
                      v-model="showModal"
                      title="Thông tin tài khoản"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer"
                    >
                      <div class="row">
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Tên tài khoản</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="userName"
                                v-model.trim="model.userName"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tài khoản"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.userName.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.userName.required"
                                class="invalid-feedback"
                            >
                              Tên tài khoản không được trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left" >Mật khẩu</label>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="password"
                                v-model="model.password"
                                type="password"
                                class="form-control"
                                placeholder="Nhập mật khẩu"
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Họ</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="lastName"
                                v-model="model.lastName"
                                type="text"
                                class="form-control"
                                placeholder="Nhập họ"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.lastName.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.lastName.required"
                                class="invalid-feedback"
                            >
                              Họ không được trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Tên</label>
                            <span style="color: red">&nbsp;*</span>
                            <input
                                id="firstName"
                                v-model="model.firstName"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tên"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.firstName.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.firstName.required"
                                class="invalid-feedback"
                            >
                              Tên không được trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Số điện thoại</label>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="phoneNumber"
                                v-model="model.phoneNumber"
                                type="text"
                                class="form-control"
                                placeholder="Nhập số điện thoại"
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Email</label>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="email"
                                v-model="model.email"
                                type="email"
                                pattern="[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}$"
                                class="form-control"
                                placeholder="Nhập email"
                            />
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Vai trò</label>
                            <span style="color: red">&nbsp;*</span>
                            <multiselect
                                        id="role"
                                        v-model="model.role"
                                         :options="listRole"
                                         label="name"
                                         :multiple="true"
                                         selectLabel="Nhấn vào để chọn"
                                         deselectLabel="Nhấn vào để xóa"
                                         track-by="id"
                                         :class="{
                                'is-invalid':
                                  submitted && $v.model.role.$error,
                              }"
                                         placeholder="Chọn vai trò"
                                         ></multiselect>
                                         
                            <div
                                v-if="submitted && !$v.model.role.required"
                                class="invalid-feedback"
                            >
                              Vai trò không được trống.
                            </div>
                          </div>
                        </div>
                        
                      </div>
                      <div class="text-end pt-2">
                        <b-button variant="light" class="w-md" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button  type="submit" variant="primary" class="ms-1 w-md">Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>
            </div>
            <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <div id="tickets-table_length" class="dataTables_length">
                      <label class="d-inline-flex align-items-center">
                        Hiện
                        <b-form-select
                            class="form-select form-select-sm"
                            v-model="perPage"
                            size="sm"
                            :options="pageOptions"
                        ></b-form-select
                        >&nbsp;dòng
                      </label>
                    </div>
                  </div>
                </div>
                <div class="table-responsive mb-0">
                  <b-table
                      class="datatables"
                      :items="myProvider"
                      :fields="fields"
                      responsive="sm"
                      :per-page="perPage"
                      :current-page="currentPage"
                      :sort-by.sync="sortBy"
                      :sort-desc.sync="sortDesc"
                      :filter="filter"
                      :filter-included-fields="filterOn"
                      ref="tblList"
                      primary-key="id"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    <template v-slot:cell(role)="row">
                      <div v-for="(item,index) in row.value" :key="index">
                              <span>
                                {{item.name}}
                              </span>
                      </div>
                    </template>
                    
                    <template v-slot:cell(donVi)="data">
                      <div v-if="data.item.donVi">
                        <p class="text-white">{{ data.item.donVi.ten }}</p>
                      </div>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-delete btn-sm"
                          v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt"></i>
                      </button>
                    </template>
                  </b-table>
                </div>
                <div class="row">
                  <b-col>
                    <div>Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <!-- pagination -->
                        <b-pagination
                            class="pagination-rounded"
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                            size="sm"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>

              </div>
            </div>
            <b-modal
                v-model="showDeleteModal"
                centered
                title="Xóa dữ liệu"
                title-class="font-18"
                no-close-on-backdrop
            >
              <p>
                Dữ liệu xóa sẽ không được phục hồi!
              </p>
              <template #modal-footer>
                <button v-b-modal.modal-close_visit
                        class="btn btn-outline-info m-1"
                        v-on:click="showDeleteModal = false">
                  Đóng
                </button>
                <button v-b-modal.modal-close_visit
                        class="btn btn-danger btn m-1"
                        v-on:click="handleDelete">
                  Xóa
                </button>
              </template>
            </b-modal>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style>
.td-stt {
  text-align: center;
  width: 55px;
}
.td-username {
  text-align: center;
  width: 120px;
}
.td-ten {
  text-align: center;
  width: 140px;
}
.td-email {
  text-align: center;
  width: 120px;
}
.td-donVi{
  text-align: center;
  width: 180px;
}
.td-xuly {
  text-align: center;
  width: 80px;
}
.td-sodienthoai {
  text-align: center;
  width: 100px;
}
</style>
