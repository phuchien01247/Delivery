<script>
import Layout from "../../layouts/main";
import {required} from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import {notifyModel} from "@/models/notifyModel";
import {pagingModel} from "@/models/pagingModel";
import {CONSTANTS} from "@/helpers/constants";
import {tagModel} from "@/models/tagModel";

export default {
  page: {
    title: "Tin tức - Sự kiện",
    meta: [{name: "description", content: appConfig.description}],
  },
  components: {Layout},
  data() {
    return {
      title: "Tin tức - Sự kiện",
      data: [],
      showModal: false,
      showDetail: false,
      showDeleteModal: false,
      submitted: false,
      pagination: pagingModel.baseJson(),
      totalRows: 1,
      todoTotalRows: 1,
      currentPage: 1,
      numberOfElement: 1,
      perPage: 10,
      pageOptions: [5,10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "age",
      sortDesc: false,
      fields: [
        { key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: {width: '30px', minWidth: '30px'}
        },
        {
          key: "title",
          label: "Tên",
          sortable: true,
        },
        {
          key: "category",
          label: "Thể loại",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '120px', minWidth: '120px'},
        },
        {
          key: "status",
          label: "Trạng thái",
          class: 'td-xuly',
          sortable: true,
          thStyle: {width: '100px', minWidth: '100px'},
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
    };
  },
  validations: {
    model: {
      name: {required},
      sort: {required}
    },
  },
  created() {
    this.fnGetList();
  },
  watch: {
    showModal(status) {
      if (status == false) this.model = tagModel.baseJson();
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
    myProvider (ctx) {
      const params = {
        start: ctx.currentPage,
        limit: ctx.perPage,
        content: this.filter,
        sortBy: ctx.sortBy,
        sortDesc: ctx.sortDesc,
        code: "TTSK"
      }
      this.loading = true
      try {
        let promise =  this.$store.dispatch("postStore/getPagingParams", params)
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
    clearSearch(){
      this.filter = null;
    },
    handleNewPost(){
      this.$router.push("/soan-bai-viet")
    },
    handleRedirectToDetail(id){
      this.$router.push("/soan-bai-viet/" + id)
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
                <h4 class="font-size-18 fw-bold cs-title-page">Tin tức - Sự kiện</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light"
                          class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button
                    variant="primary"
                    type="button"
                    class="btn w-md btn-primary"
                    @click="handleNewPost"
                    size="sm"
                >
                  <i class="mdi mdi-plus me-1"></i> Tạo bài viết
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
                      <input
                          v-model = "filter"
                          type="text"
                          class="form-control"
                          placeholder="Tìm kiếm ..."
                      />
                      <i class="bx bx-search-alt search-icon"></i>
                    </div>
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
            <div class="row">
              <div class="col-12">
                <div class="row mb-3">
                  <div class="col-sm-12 col-md-6">
                    <div id="tickets-table_length" class="dataTables_length">
                      <label class="d-inline-flex align-items-center cs-text-primary">
                        Hiện
                        <b-form-select
                            class="form-control form-control-sm form-select form-select-sm cs-form-select"
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
                      class="datatables custom-table"
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
                      :busy.sync="isBusy"
                      tbody-tr-class="b-table-chucvu"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage-1)*perPage) + 1  }}
                    </template>
                    <template v-slot:cell(category)="data">
                      <template v-if="data.item.category">
                        {{data.item.category.name}}
                      </template>
                    </template>
                    <template v-slot:cell(status)="data">
                      <template v-if="data.item.status">
                        {{data.item.status.name}}
                      </template>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button
                          type="button"
                          size="sm"
                          class="btn btn-edit btn-sm"
                          data-toggle="tooltip" data-placement="bottom" title="Xem chi tiết"
                          v-on:click="handleRedirectToDetail(data.item.id)">
                        <i class="fas fas fa-eye"></i>
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
                    <div class="cs-text-primary">Hiển thị {{numberOfElement}} trên tổng số {{totalRows}} dòng</div>
                  </b-col>
                  <div class="col">
                    <div
                        class="dataTables_paginate paging_simple_numbers float-end">
                      <ul class="pagination pagination-sm mb-0">
                        <b-pagination
                            v-model="currentPage"
                            :total-rows="totalRows"
                            :per-page="perPage"
                            class="pagination-sm cs-pagination"
                        ></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
        <!--        <b-modal-->
        <!--            v-model="showDeleteModal"-->
        <!--            centered-->
        <!--            title="Xóa dữ liệu"-->
        <!--            title-class="font-18"-->
        <!--            no-close-on-backdrop-->
        <!--        >-->
        <!--          <p>-->
        <!--            Dữ liệu xóa sẽ không được phục hồi!-->
        <!--          </p>-->
        <!--          <template #modal-footer>-->
        <!--            <b-button v-b-modal.modal-close_visit-->
        <!--                      size="sm"-->
        <!--                      class="btn btn-outline-info w-md"-->
        <!--                      v-on:click="showDeleteModal = false">-->
        <!--              Đóng-->
        <!--            </b-button>-->
        <!--            <b-button v-b-modal.modal-close_visit-->
        <!--                      size="sm"-->
        <!--                      variant="danger"-->
        <!--                      type="button"-->
        <!--                      class="w-md"-->
        <!--                      v-on:click="handleDelete">-->
        <!--              Xóa-->
        <!--            </b-button>-->
        <!--          </template>-->
        <!--        </b-modal>-->
      </div>
    </div>
  </Layout>
</template>
<style>
.td-stt {
  text-align: center;
}
.td-xuly {
  text-align: center;
}
.table>tbody> tr >td{
  padding: 0px;
  line-height: 30px;
}
.hidden-sortable:after, .hidden-sortable:before {
  display: none !important;
}

@media only screen and (max-width: 768px) {
  .b-table-chucvu>td:nth-of-type(1):before{
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
    content: "Thứ tự";
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
