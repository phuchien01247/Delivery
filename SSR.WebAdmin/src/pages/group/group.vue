<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import {numeric, required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import Multiselect from "vue-multiselect";
import {groupModel} from "@/models/groupModel";

export default {
  page: {
    title: "Quản lý nhóm người dùng",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout, Multiselect},
  data() {
    return {
      title: "Quản lý nhóm người dùng",
      items: [
        {
          text: "group",
          href:"/group",
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
          key: "name",
          label: "Tên nhóm",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '80px', minWidth: '80px'},
        },
        {
          key: "description",
          label: "Miêu tả",
          class: 'td-ten',
          sortable: true,
        },
        {
          key: "members",
          label: "Thành viên",
          class: 'td-ten',
          sortable: true,
          thStyle: {width: '100px', minWidth: '100px'},
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: {width: '50px', minWidth: '50px'},
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
      model: groupModel.baseJson(),
      listUser: [],
      pagination: pagingModel.baseJson(),
      itemFilter:{
        code: null,
        name: null,
      }
    };
  },
  validations: {
    model: {
      name: {required},
      description: {required},
      members: {required}
    },
  },
  methods: {
    handleSearch() {
      this.fnGetList()
    },
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
    async getListUser(){
      await  this.$store.dispatch("userStore/getAll").then((res) =>{
        this.listUser = res.data || [];
      })
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("groupStore/delete", this.model.id).then((res) => {
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
          await this.$store.dispatch("groupStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.model= groupModel.baseJson();
              this.$refs.tblList.refresh();
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("groupStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList(); 
              this.model= groupModel.baseJson();
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
      await this.$store.dispatch("groupStore/getbyid", id).then((res) => {
        if (res.resultCode==='SUCCESS') {
          this.model = groupModel.toJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
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
        let promise =  this.$store.dispatch("groupStore/getPagingParams", params)
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
    this.getListUser();
  },
  mounted() {

  },

  watch: {
    model: {
      deep: true,
      handler(val) {}
    },
    showModal(status) {
      if (status == false) this.model = groupModel.baseJson();
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
              <h4 class="font-size-18 fw-bold cs-title-page">Nhóm</h4>
            </div>
            <div class="col-md-8 col-12 text-end">
              <b-button
                  type="button"
                  variant="primary"
                  class="w-md"
                  size="sm"
                  @click="showModal = true"
              >
                <i class="mdi mdi-plus me-1"></i> Tạo nhóm
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
                      title="Thông tin group"
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
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên nhóm</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="name"
                                v-model.trim="model.name"
                                type="text"
                                class="form-control"
                                placeholder="Nhập tên nhóm"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.name.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.name.required"
                                class="invalid-feedback"
                            >
                              Tên nhóm không được trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left" >Mô tả</label>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                id="description"
                                v-model="model.description"
                                type="text"
                                class="form-control"
                                placeholder="Nhập mô tả"
                            />
                          </div>
                        </div>
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Thành viên</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <multiselect 
                              id="members"
                              v-model="model.members"
                              :options="listUser"
                              label="fullName"
                              :multiple="true"
                              selectLabel="Nhấn vào để chọn"
                              deselectLabel="Nhấn vào để xóa"
                              track-by="id"
                              :class="{
                                'is-invalid':
                                submitted && $v.model.members.$error,
                              }"
                              placeholder="Chọn thành viên"></multiselect>
                            <div
                                v-if="submitted && !$v.model.members.required"
                                class="invalid-feedback">
                              Thành viên không được trống.
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
                    <template v-slot:cell(name)="data">
                      {{data.item.name}}
                    </template>
                    <template v-slot:cell(description)="data">
                      {{data.item.description}}
                    </template>
                    <template v-slot:cell(members)="data">
                        <span class="badge rounded-pill" style="background-color: #007bbc;" v-for="(value , index) in data.item.members" :key="index">
                          {{value.fullName}}
                        </span>
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
