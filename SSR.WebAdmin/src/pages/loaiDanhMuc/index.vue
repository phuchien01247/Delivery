<script>
import Layout from "../../layouts/main";
import { required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import { notifyModel } from "@/models/notifyModel";
import { pagingModel } from "@/models/pagingModel";
import { CONSTANTS } from "@/helpers/constants";
import { categoryModel } from "@/models/categoryModel";
import { loaiDanhMucModel } from "@/models/loaiDanhMucModel";
import { knowledgeModel } from "@/models/knowledgeModel";
import Multiselect from "vue-multiselect";
import Switches from "vue-switches";

export default {
  page: {
    title: "Nhãn",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: { Layout, Multiselect, Switches},
  data() {
    return {
      title: "Loại danh mục",
      data: [],
      showModal: false,
      showPhanloai: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: loaiDanhMucModel.baseJson(),
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5, 10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "age",
      sortDesc: false,
      listKnowledge: [],
      fields: [
        {
          key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: { width: '30px', minWidth: '30px' }
        },
        {
          key: "name",
          label: "Tên loại",
          sortable: true,
        },
        {
          key: "color",
          label: "Màu sắc",
          class: 'td-xuly',
          sortable: true,
          thStyle: { width: '100px', minWidth: '100px' },
        },
        {
          key: "knowledge",
          label: "Hướng dẫn",
          class: 'td-xuly',
          sortable: true,
          thStyle: { width: '130px', minWidth: '130px' },
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          sortable: false,
          thStyle: { width: '130px', minWidth: '130px' },
        }
      ],
    };
  },
  validations: {
    model: {
      knowledge: { required },
      name: { required },
      color: { required },
      isglobal: { required },
    },
  },
  created() {
    this.fnGetList();
    this.getListKnowledge();
  },
  watch: {
    showModal(status) {
      if (status == false) this.model = loaiDanhMucModel.baseJson();
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  methods: {
    handleSearch() {
      this.fnGetList()
    },
    fnGetList() {
      this.$refs.tblList?.refresh()
    },
    async getListKnowledge() {
      await this.$store.dispatch("knowledgeStore/get").then((res) => {
        this.listKnowledge = res.data || [];
      })
    },
    async handleUpdate(id) {
      await this.$store.dispatch("loaiDanhMucStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
          this.showModal = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.fnGetList();
        }
      });
    },
    async handleDetail(id) {
      await this.$store.dispatch("loaiDanhMucStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("loaiDanhMucStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.fnGetList();
            this.showDeleteModal = false;
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
          await this.$store.dispatch("loaiDanhMucStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("loaiDanhMucStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.getListKnowledge();
              this.showModal = false;
              this.model = loaiDanhMucModel.baseJson();
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    myProvider(ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
      }
      this.loading = true
      try {
        let promise = this.$store.dispatch("loaiDanhMucStore/getPagingParams", params)
        return promise.then(resp => {
          if (resp.resultCode == CONSTANTS.SUCCESS) {
            let data = resp.data;
            this.totalRows = data.totalRows
            let items = data.data
            this.numberOfElement = items.length
            this.loading = false
            return items || []
          } else {
            return [];
          }
        })
      } finally {
        this.loading = false
      }
    },
    clearSearch() {
      this.filter = null;
    }
  }
}
</script>
<template>
  <Layout>
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold cs-title-page">PHÂN LOẠI NHÃN</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light" class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button variant="primary" type="button" class="btn w-md btn-primary" @click="showModal = true"
                  size="sm">
                  <i class="mdi mdi-plus me-1"></i> Tạo phân loại nhãn
                </b-button>
              </div>
            </div>
            <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Nội dung</label>
                      <input v-model="filter" type="text" class="form-control" placeholder="Tìm kiếm ..." />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="handleSearch" variant="light" class="btn w-md btn-primary me-2" size="md">
                            <i class="fas fa-search align-middle me-2"></i>
                            Tìm kiếm
                          </b-button>
                        </div>
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light" class="btn w-md btn-secondary me-2" size="md">
                            <i class="fas fa-redo-alt align-middle me-2"></i>
                            Làm mới
                          </b-button>
                        </div>
                      </div>
                    </div>
                  </div>
                </div>
              </div>
            </b-collapse>
          </div>
        </div>
      </div>
    </div>
    <div class="row">
      <div class="col-12">
        <div class="card">
          <div class="card-body">
            <div class="row mb-2">
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal v-model="showModal" title="Thông tin loại nhãn" title-class="text-black font-18"
                    body-class="p-3" hide-footer centered no-close-on-backdrop size="lg">
                    <form @submit.prevent="handleSubmit" ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên phân loại nhãn</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id" />
                            <input id="name" v-model.trim="model.name" type="text" class="form-control"
                              placeholder="Nhập tên nhãn" :class="{
                                'is-invalid':
                                  submitted && $v.model.name.$error,
                              }" />
                            <div v-if="submitted && !$v.model.name.required" class="invalid-feedback">
                              Phân loại nhãn không được để trống.
                            </div>
                          </div>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-12">
                          <label class="text-left">Chọn hướng dẫn</label>
                          <span style="color: red">&nbsp;*</span>
                          <multiselect v-model="model.knowledge" :options="listKnowledge" track-by="id" label="name"
                            placeholder="Chọn hướng dẫn" deselect-label="Nhấn để xoá" selectLabel="Nhấn enter để chọn"
                            selectedLabel="Đã chọn" :multiple="true"></multiselect>
                          <multiselect v-model="selected" :options="options" :searchable="false" :close-on-select="false"
                            :allow-empty="false" placeholder="Select one" label="name" track-by="name">
                          </multiselect>
                        </div>
                      </div>
                      <div class="row">
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Chọn màu phân loại nhãn</label>
                            <span style="color: red">&nbsp;*</span>
                            <br>
                            <input type="hidden" v-model="model.id" />
                            <input id="color" v-model="model.color" type="color" min="0"
                              oninput="validity.valid||(value='');" class="choosecolor" :class="{
                                'is-invalid':
                                  submitted && $v.model.color.$error,
                              }" />
                            <div v-if="submitted && !$v.model.color.required" class="invalid-feedback">
                              Màu không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-6">
                          <div class="mb-3">
                            <label class="text-left">Dùng chung</label>
                            <br />
                            <switches class="mt-2 mb-0"  v-model="model.isglobal" type-bold="true" color="success"  value="true" >
                            </switches>
                            <v-switch label="on disabled" :value="true" disabled></v-switch>
                          </div>
                        </div>
                      </div>
                      <div class="text-end pt-2 mt-3">
                        <b-button variant="light" class="w-md" size="sm" @click="showModal = false">
                          Đóng
                        </b-button>
                        <b-button type="submit" variant="primary" size="sm" class="ms-1 w-md">
                          Lưu
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
                        <b-form-select class="form-select form-select-sm" v-model="perPage" size="sm"
                          :options="pageOptions"></b-form-select>&nbsp;dòng
                      </label>
                    </div>
                  </div>
                </div>
                <div class="table-responsive-sm">
                  <b-table class="datatables custom-table" :items="myProvider" :fields="fields" responsive="sm"
                    :per-page="perPage" :current-page="currentPage" :sort-by.sync="sortBy" :sort-desc.sync="sortDesc"
                    :filter="filter" :filter-included-fields="filterOn" ref="tblList" primary-key="id" :busy.sync="isBusy"
                    tbody-tr-class="b-table-chucvu">
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage - 1) * perPage) + 1 }}
                    </template>
                    <template v-slot:cell(name)="data">
                      <template v-if="data.item.name">
                        <div>
                          <span class="colorstyle" v-bind:style="{ background: data.item.color }">{{ data.item.name }}</span>
                        </div>
                      </template>
                    </template>
                    <template v-slot:cell(knowledge)="data">
                      <div v-for="(value , index) in data.item.knowledge" :key="index">
                        <span  class="badge bg-success ms-1"> {{value.name}}</span>
                      </div>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button type="button" size="sm" class="btn btn-edit btn-sm" data-toggle="tooltip"
                        data-placement="bottom" title="Cập nhật" v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt"></i>
                      </button>
                      <button type="button" size="sm" class="btn btn-delete btn-sm" data-toggle="tooltip"
                        data-placement="bottom" title="Xóa" v-on:click="handleShowDeleteModal(data.item.id)">
                        <i class="fas fa-trash-alt"></i>
                      </button>
                    </template>
                  </b-table>
                  <template v-if="isBusy">
                    <div align="center">Đang tải dữ liệu</div>
                  </template>
                  <template v-if="totalRows <= 0 && !isBusy">
                    <div align="center">Không có dữ liệu</div>
                  </template>
                </div>
                <div class="row">
                  <b-col>
                    <div>Hiển thị {{ numberOfElement }} trên tổng số {{ totalRows }} dòng</div>
                  </b-col>
                  <div class="col">
                    <div class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-rounded mb-0">
                        <b-pagination v-model="currentPage" :total-rows="totalRows" :per-page="perPage"
                          size="sm"></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <b-modal v-model="showDeleteModal" centered title="Xóa dữ liệu" title-class="font-18" no-close-on-backdrop>
          <p>
            Dữ liệu xóa sẽ không được phục hồi!
          </p>
          <template #modal-footer>
            <b-button v-b-modal.modal-close_visit size="sm" class="btn btn-outline-info w-md"
              v-on:click="showDeleteModal = false">
              Đóng
            </b-button>
            <b-button v-b-modal.modal-close_visit size="sm" variant="danger" type="button" class="w-md"
              v-on:click="handleDelete">
              Xóa
            </b-button>
          </template>
        </b-modal>
      </div>
    </div>
  </Layout>
</template>

<style>
.choosecolor {
  appearance: none;
  width: 60px;
  height: 30px;
  background-color: transparent;
  border: none;
  cursor: pointer;
}

.colorstyle {
  padding: 5px;
  border-radius: 5px;
  font-weight: bold;
  color: white
}

.td-stt {
  text-align: center;
}

.td-xuly {
  text-align: center;
}

.table>tbody>tr>td {
  padding: 0px;
  line-height: 30px;
}

.table>tbody>tr>td>div {
  color: #00568c;
}

.hidden-sortable:after,
.hidden-sortable:before {
  display: none !important;
}

@media only screen and (max-width: 768px) {
  .b-table-chucvu>td:nth-of-type(1):before {
    content: "STT";
    font-weight: bold;
    color: #00568c;
  }

  .b-table-chucvu>td:nth-of-type(2):before {
    content: "Tên";
    font-weight: bold;
    color: #00568c;
  }

  .b-table-chucvu>td:nth-of-type(3):before {
    content: "Màu sắc";
    font-weight: bold;
    color: #00568c;
  }

  .b-table-chucvu>td:nth-of-type(4):before {
    content: "";
    font-weight: bold;
    color: #00568c;
  }
}
</style>

