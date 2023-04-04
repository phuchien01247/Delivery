<script>
import Layout from "../../layouts/main";
import PageHeader from "@/components/page-header";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import DatePicker from "vue2-datepicker";

export default {
  page: {
    title: "Quản Lý Logging",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout,DatePicker},
  data() {
    return {
      title: "Lịch sử thao tác",
      items: [
        {
          text: "Logging",
          href: "/logging",
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      fields: [
        {
          key: 'STT',
          label: 'STT',
          class : "text-center",
          thStyle: {width: '50px', minWidth: '50px' },
          tdClass: 'align-middle',
          thClass: 'hidden-sortable'
        },
        {
          key: "lastModifiedShow",
          label: "Ngày",
          class : "text-center",
          sortable: true,
          thStyle: {width: '110px', minWidth: '110px'},
        },
        {
          key: "modifiedBy",
          label: "UserName",
          sortable: true,
        },
        {
          key: "collectionName",
          class : "text-center",
          label: "Collection Name",
          sortable: true,
        },
        {
          key: "action",
          label: "Action",
          class : "text-center",
          thStyle: {width: '100px', minWidth: '100px'},
          thClass: 'hidden-sortable'
        },
        {
          key: "actionResult",
          label: "ActionResult",
          class : "text-center",
          thStyle: {width: '130px', minWidth: '130px'},
          thClass: 'hidden-sortable'
        },
        {
          key: "contentLog",
          label: "ConnentLog",
          thClass: 'hidden-sortable',
          class : "text-center",
          sortable: true,
        }
      ],
      itemFilter: {
        title: null,
        ngayBatDau: null,
        ngayKetThuc: null,
      },
    };
  },
  validations: {
    model: {
      ten: {required},
      thuTu: {required}
    },
  },
  watch: {
    model: {
      deep: true,
      handler(val) {
        // addCoQuanToModel()
        // this.saveValueToLocalStorage()
      }
    },

    showDeleteModal(val) {
      if (val == false) {
        this.model.id = null;
      }
    },
  },
  methods: {
    clearSearch() {
      this.itemFilter.title = null;
      this.itemFilter.ngayBatDau = null;
      this.itemFilter.ngayKetThuc = null;
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
        let promise =  this.$store.dispatch("loggingStore/getPagingParams", params)
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
    }
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
                <h4 class="font-size-18 fw-bold text-dark">Lịch sử thao tác</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button
                    v-if="isVanThu"
                    variant="primary"
                    class="btn w-md btn-primary"
                    @click="handleCreate"
                    size="sm"
                >
                  <i class="mdi mdi-plus align-middle me-2"></i> Nhập văn bản
                </b-button>
              </div>
            </div>
            <b-collapse id="collapseSearch" class="mt-1">
              <div class="row">
                <div class="col-12">
                  <div class="d-flex justify-content-between align-items-end flex-wrap mb-2">
                    <div class="flex-grow-1 me-2">
                      <label class="mb-1">Ngày bắt đầu</label>
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 me-2 mb-md-0 mb-2">
                          <date-picker v-model="itemFilter.ngayBatDau"
                                       format="DD/MM/YYYY"
                                       value-type="format"
                          >
                            <div slot="input">
                              <input v-model="itemFilter.ngayBatDau"
                                     v-mask="'##/##/####'" type="text" class="form-control"
                                     placeholder="Nhập ngày bắt đầu"/>
                            </div>
                          </date-picker>
                        </div>
                        <div class="flex-grow-1 me-2 mb-md-0 mb-2">
                          <date-picker v-model="itemFilter.ngayKetThuc"
                                       format="DD/MM/YYYY"
                                       value-type="format"
                          >
                            <div slot="input">
                              <input v-model="itemFilter.ngayKetThuc"
                                     v-mask="'##/##/####'" type="text" class="form-control"
                                     placeholder="Nhập ngày kết thúc"/>
                            </div>
                          </date-picker>
                        </div>
                      </div>
                    </div>
                    <!-- Nội dung -->
                    <div class="flex-grow-1 me-2">
                      <label>Nội dung</label>
                      <input
                          size="sm"
                          type="text"
                          name="untyped-input"
                          class="form-control"
                          v-model="itemFilter.title"
                          placeholder="Trích yếu/Số lưu CV/..."
                      />
                    </div>
                    <!--  Xử lý -->
                    <div class="flex-grow-0 ms-2">
                      <div class="d-flex justify-content-between align-items-center flex-wrap">
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="handleSearch" variant="light"
                                    class="btn w-md btn-primary me-2" size="md">
                            <i class="fas fa-search align-middle me-2"></i>
                            Tìm kiếm
                          </b-button>
                        </div>
                        <div class="flex-grow-1 mt-xl-0 mt-2">
                          <b-button @click="clearSearch" variant="light"
                                    class="btn w-md btn-secondary me-2" size="md">
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
                          striped
                          bordered
                          responsive="sm"
                          :per-page="perPage"
                          :current-page="currentPage"
                          :filter="filter"
                          ref="tblList"
                          primary-key="id"
                      >
                        <template v-slot:cell(STT)="data">
                          {{ data.index + ((currentPage-1)*perPage) + 1  }}
                        </template>
                        <template v-slot:cell(ten)="data">&nbsp;&nbsp;
                          {{data.item.ten}}
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
.table>tbody> tr >td{
  padding: 0px;
  line-height: 30px;
}
.hidden-sortable:after, .hidden-sortable:before {
  display: none !important;
}

</style>
