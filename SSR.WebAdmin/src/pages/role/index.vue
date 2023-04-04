<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {roleModel} from "@/models/roleModel";
import {CONSTANTS} from "@/helpers/constants";
export default {
  page: {
    title: "Quản lý vai trò",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout},
  data() {
    return {
      title: "Quản lý vai trò",
      items: [
        {
          text: "Vai trò",
          href:"/vai-tro",
          // active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: roleModel.baseJson(),
      listRole: [],
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      todoFilter: null,
      filterOn: [],
      todofilterOn: [],
      sortBy: "age",
      sortDesc: false,
      fields: [
        { key: 'STT', label: 'STT',
          class: "text-center",
          thStyle: {width: '80px', minWidth: '60px' },
          tdClass: 'align-middle',
          thClass: 'hidden-sortable'},
        
        {
          key: "name",
          label: "Tên quyền",
          sortable: true,
          thStyle: {width: '300px', minWidth: '150px'},
        },
        {
          key: "permissions",
          label: "Số lượng",
          class : "text-center",
          tdClass: 'align-middle',
          thClass: 'hidden-sortable'
        },
        {
          key: 'process',
          label: 'Xử lý',
          class : "text-center",
          thStyle: {width: '150px', minWidth: '150px'},
          thClass: 'hidden-sortable'
        }
      ],
      itemFilter:{
        code: null,
        name: null,
      }
    };
  },
  validations: {
    model: {
      name: {required},

    },
  },
  created() {
  },
  watch: {
    model: {
      deep: true,
      handler(val) {
        // addCoQuanToModel()
        // this.saveValueToLocalStorage()
      }
    },
    showModal(status) {
      if (status == false) this.model  = roleModel.baseJson();
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  methods: {
    clearSearch(){
      this.itemFilter.code= null;
      this.itemFilter.name= null;
    },
    async handleUpdate(id) {
      await this.$store.dispatch("roleStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = roleModel.fromJson(res.data);
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.$refs.tblList.refresh()
        }
      });
    },
    async handleDetail(id) {
      await this.$store.dispatch("roleStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = roleModel.fromJson(res.data);
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("roleStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.showDeleteModal = false;
            this.$refs.tblList.refresh()
          }
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          // });
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
          await this.$store.dispatch("roleStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.$refs.tblList.refresh()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("roleStore/create", roleModel.toJson(this.model)).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.$refs.tblList.refresh()
              this.model = {};
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
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
        let promise =  this.$store.dispatch("roleStore/getPagingParams", params)
        return promise.then(resp => {
          if(resp.resultCode == CONSTANTS.SUCCESS){
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            return items || []
          }else{
            return [];
          }
        })
      } finally {
        this.loading = false
      }
    },
    // todoFiltered(filteredItems) {
    //   // Trigger pagination to update the number of buttons/pages due to filtering
    //   this.todoTotalRows = filteredItems.length;
    //   this.todocurrentPage = 1;
    // }
  }
}
</script>
<template>
  <Layout>
<!--    <PageHeader :title="title" :items="items"/>-->
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold cs-title-page">Quản lý vai trò</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button type="button" variant="primary" class="btn-label w-md"  @click="showModal = true" size="sm"
                >
                  <i class="mdi mdi-plus me-1 label-icon"></i> Thêm vai trò
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
                      title="Thông tin quyền"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên quyền</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id"/>
                            <input
                                v-model="model.name"
                                type="text"
                                min="0"
                                oninput="validity.valid||(value='');"
                                class="form-control"
                                placeholder="Nhập tên quyền"
                                :class="{
                                'is-invalid':
                                  submitted && $v.model.name.$error,
                              }"
                            />
                            <div
                                v-if="submitted && !$v.model.name.required"
                                class="invalid-feedback"
                            >
                              Tên quyền không được để trống.
                            </div>
                          </div>
                        </div>
                        
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button type="submit" variant="success" class="ms-1">
                          Lưu
                        </b-button>
                      </div>
                    </form>
                  </b-modal>
                  <b-modal
                      v-model="showDetail"
                      title="Thông tin chi tiết vai trò"
                      title-class="text-black font-18"
                      body-class="p-3"
                      hide-footer
                      centered
                      no-close-on-backdrop
                      size="lg"
                  >
                    <form @submit.prevent="handleSubmit"
                          ref="formContainer">
                      <div class="row">
                        
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên quyền : </label>
                            <input
                                v-model="model.name"
                                type="text"
                                class="form-control"
                            />
                          </div>
                        </div>
                        
                       
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" @click="showDetail = false">
                          Đóng
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
                <div class="table-responsive-sm">
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
                    <template v-slot:cell(permissions)="data">
                      <router-link :to='`/vai-tro/thiet-lap-quyen/${data.item.id}`'>
                        <b-button
                            v-if="data.item.permissions.length > 0 " variant="btn-sm"
                            class="bg-success text-white"
                            style="border-radius: 3px;"
                        >
                            {{ data.item.permissions.length}}
                        </b-button>
                        <b-button v-else  variant="btn-sm"
                                  class="bg-success text-white"
                                  style="border-radius: 3px;"
                        >
                            {{0}}
                        </b-button>
                      </router-link>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-detail btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Chi tiết"
                          v-on:click="handleDetail(data.item.id)">
                        <i class="fas fa-eye"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Cập nhật"
                          v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-delete btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Xóa"
                          v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt"></i>
                      </button>
                    </template>
                    <template v-slot:cell(name)="data">&nbsp;&nbsp;
                      {{data.item.name}}
                    </template>
                    <!-- <template v-slot:cell(permissions)="data">
                      <div v-for="(value , index) in data.item.permissions" :key="index">
                        <span  class="badge bg-success ms-1"> {{value.name}}</span>
                      </div>
                    </template> -->
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
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
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
  </Layout>
</template>
<style>
.hidden-sortable:after, .hidden-sortable:before {
  display: none !important;
}
.table>tbody> tr >td{
  padding: 0px;
  line-height: 30px;
}

</style>
