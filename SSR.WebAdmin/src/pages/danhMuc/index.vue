<script>
import Layout from "../../layouts/main";
import { required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import { notifyModel } from "@/models/notifyModel";
import { pagingModel } from "@/models/pagingModel";
import { CONSTANTS } from "@/helpers/constants";
import { knowledgeModel } from "@/models/knowledgeModel";

export default {
  page: {
    title: "Hướng dẫn xử lý",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: { Layout, 'ckeditor-nuxt': () => { return import('@blowstack/ckeditor-nuxt') }, },
  data() {
    return {
      editorConfig: {
        toolbar: {
          items: [
            'heading', '|',
            'fontfamily', 'fontsize', '|',
            'uploadImage',
            'code', 'codeBlock', '|',
            'alignment', '|',
            'fontColor', 'fontBackgroundColor', '|',
            'bold', 'italic', 'strikethrough', 'underline', 'subscript', 'superscript', '|',
            'link', '|',
            'outdent', 'indent', '|',
            'bulletedList', 'numberedList', 'todoList', '|',
            'insertTable', '|',
            'undo', 'redo'
          ],
          shouldNotGroupWhenFull: false
        },
        removePlugins: ['Title', 'ImageCaption'],
        simpleUpload: {
          uploadUrl: process.env.VUE_APP_API_URL + "files/upload-ckeditor",
          headers: {
            'Authorization': 'optional_token'
          }
        },
      },

      title: "Hướng dẫn xử lý lỗi",
      data: [],
      showModal: false,
      showModalXemchitiet: false,
      showPhanloai: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      model: knowledgeModel.baseJson(),
      pagination: pagingModel.baseJson(),
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
          label: "Tiêu đề",
          sortable: true,
        },
        {
          key: "summary",
          label: "Mô tả ngắn",
          sortable: true,
          thStyle: { width: '750px', minWidth: '750px' },
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
      listLoaiDanhMuc: []
    };
  },
  validations: {
    model: {
      name: { required },
      summary: { required },
      content: { required },
    },
  },
  created() {
    this.fnGetList();
    this.getListKnowledge();
    this.stripHtml();
  },
  watch: {
    showModal(status) {
      if (status == false) this.model = knowledgeModel.baseJson();
    },
    showModalXemchitiet(status) {
      if (status == false) this.model = knowledgeModel.baseJson();
    },
    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  mounted() {
    this.plainText = this.$refs.myText.innerText;
  },
  methods: {
    stripHtml() {
      const value = this.model.summary;
      const div = document.createElement('div');
      div.innerHTML = value;
      const text = div.textContent || div.innerText || '';
      return text;
    },
    handleSearch() {
      this.fnGetList()
    },
    fnGetList() {
      this.$refs.tblList?.refresh()
    },
    async getListKnowledge() {
      await this.$store.dispatch("knowledgeStore/get").then((res) => {
        this.listLoaiDanhMuc = res.data || [];
      })
    },
    async handleUpdate(id) {
      await this.$store.dispatch("knowledgeStore/getById", id).then((res) => {
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
      await this.$store.dispatch("knowledgeStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    async handleRedirectToDetail(id) {
      await this.$store.dispatch("knowledgeStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = res.data
          this.showModalXemchitiet = true;

          const value = this.model.summary;
          const div = document.createElement('div');
          div.innerHTML = value;
          const text = div.textContent || div.innerText || '';
          return text;

        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
          this.fnGetList();
        }
      });
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("knowledgeStore/delete", this.model.id).then((res) => {
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
          await this.$store.dispatch("knowledgeStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("knowledgeStore/create", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.fnGetList();
              this.showModal = false;
              this.model = knowledgeModel.baseJson();
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
        let promise = this.$store.dispatch("knowledgeStore/getPagingParams", params)
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
                <h4 class="font-size-18 fw-bold cs-title-page">Hướng dẫn xử lý yêu cầu lỗi</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button variant="primary" type="button" class="btn w-md btn-primary" @click="showModal = true"
                  size="sm">
                  <i class="mdi mdi-plus me-1"></i> Tạo hướng dẫn xử lý
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
                    <input v-model="filter" type="text" class="form-control" placeholder="Tìm kiếm ..." />
                    <i class="bx bx-search-alt search-icon"></i>
                  </div>
                </div>
              </div>
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal v-model="showModalXemchitiet" title="Thông tin chi tiết" title-class="text-black font-18"
                    body-class="p-3" hide-footer centered no-close-on-backdrop size="lg">
                    <form @submit.prevent="handleSubmit" ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left" style="color:red">Tên hướng dẫn</label>
                            <input type="hidden" v-model="model.id" />
                            <p ref="myText" v-html="model.name"></p>
                          </div>
                        </div>
                        <div class="col-md-12">
                          <div class="mb-2">
                            <label class="text-left" style="color:red">Mô tả ngắn</label>
                            <p ref="myText" v-html="model.summary"></p>
                            <p>{{ plainText }}</p>
                          </div>
                        </div>
                        <div class="col-md-12">
                          <div class="mb-2">
                            <label class="text-left" style="color:red">Nội dung </label>
                            <p ref="myText" v-html="model.content"></p>
                            <p>{{ plainText }}</p>
                          </div>
                        </div>
                      </div>
                    </form>
                  </b-modal>
                </div>
              </div>
              <div class="col-sm-8">
                <div class="text-sm-end">
                  <b-modal v-model="showModal" title="Thông tin hướng dẫn" title-class="text-black font-18"
                    body-class="p-3" hide-footer centered no-close-on-backdrop size="lg">
                    <form @submit.prevent="handleSubmit" ref="formContainer">
                      <div class="row">
                        <div class="col-12">
                          <div class="mb-3">
                            <label class="text-left">Tên hướng dẫn</label>
                            <span style="color: red">&nbsp;*</span>
                            <input type="hidden" v-model="model.id" />
                            <input id="name" v-model.trim="model.name" type="text" class="form-control"
                              placeholder="Nhập tên hướng dẫn" :class="{
                                'is-invalid':
                                  submitted && $v.model.name.$error,
                              }" />
                            <div v-if="submitted && !$v.model.name.required" class="invalid-feedback">
                              Tên hướng dẫn không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-md-12">
                          <div class="mb-2">
                            <label class="form-label cs-title-form" for="validationCustom01">Mô tả ngắn</label>
                            <textarea class="form-control" v-model="model.summary" rows="4"
                              :class="{ 'is-invalid': submitted && $v.model.summary.$error, }"></textarea>
                            <div v-if="submitted && !$v.model.summary.required" class="invalid-feedback">
                              Trích yếu không được để trống.
                            </div>
                          </div>
                        </div>
                        <div class="col-md-12">
                          <div class="mb-2">
                            <label class="form-label cs-title-form" for="validationCustom01"> Nội dung</label>
                            <span class="text-danger">*</span>
                            <ckeditor-nuxt v-model="model.content" :config="editorConfig" />
                            <div v-if="submitted && !$v.model.content.required" class="invalid-feedback">
                              Nội dung không được để trống.
                            </div>
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
                          <span>{{ data.item.name }}</span>
                        </div>
                      </template>
                    </template>
                    <template v-slot:cell(summary)="data">
                      <div class="ellips">
                        <span>{{ data.item.summary }}</span>
                      </div>
                    </template>
                    <template v-slot:cell(content)="data">
                      <span>{{ data.item.content }}</span>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button type="button" size="sm" class="btn btn-detail btn-sm" data-toggle="tooltip"
                        data-placement="bottom" title="Xem chi tiết" v-on:click="handleRedirectToDetail(data.item.id)">
                        <i class="fas fas fa-eye"></i>
                      </button>
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
.ellipsis {
  white-space: nowrap;
  text-overflow: ellipsis;
  overflow: hidden;
}

.ellips {
  display: block;
  display: -webkit-box;
  max-width: 100%;
  margin: 0 auto;
  font-size: 14px;
  line-height: 30px;
  -webkit-line-clamp: 2;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
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

