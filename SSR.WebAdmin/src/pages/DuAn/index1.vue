<script>
import Layout from "../../layouts/main";
import appConfig from "@/app.config";
import { projectModel } from "@/models/projectModel";
import { required } from "vuelidate/lib/validators";
import { notifyModel } from "@/models/notifyModel";
import { pagingModel } from "@/models/pagingModel";
import { CONSTANTS } from "@/helpers/constants";
export default {
  page: {
    title: "Quản lý dự án",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: { Layout },
  data() {
    return {
      title: "Quản lý dự án",
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
      pageOptions: [5, 10, 25, 50, 100],
      filter: null,
      filterOn: [],
      isBusy: false,
      sortBy: "date",
      sortDesc: true,
      model: projectModel.baseJson(),
      items: [
        {
          text: "Dự án",
          href: "/du-an",
          // active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],

      fields: [
        {
          key: 'STT',
          label: 'STT',
          class: 'cs-text-center',
          sortable: false,
          thClass: 'hidden-sortable',
          thStyle: { width: '40px', minWidth: '60px' }
        },
        {
          key: "name",
          label: "Dự án",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '600px', minWidth: '200px'},
        },
        {
          key: "createdAt",
          label: "Ngày tạo",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '50px' },
        },
        {
          key: "createdBy",
          label: "Người tạo",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '90px' },
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          // sortable: false,
          thStyle: { width: '80px', minWidth: '50px' },
        }
      ],
    };
  },
  async created() {
    if (this.$route.params.id) {
      this.getById(this.$route.params.id);
    } else {
      this.model = projectModel.baseJson();
    }
    if (this.$route.params.slug) {
      this.getBySlug(this.$route.params.slug);
    } else {
      this.model = projectModel.baseJson();
    }
    this.handleGetDonVi();
  },
  watch: {
    model: {

    }
  },
  computed: {
  },
  mounted() {

  },
  validations: {
    model: {
    },
  },

  methods: {
    convertdate(date) {
      let newsApiDate = date; // got from the Api
      let timestamp = new Date(newsApiDate).getTime();
      let Day = new Date(timestamp).getDate();
      let Month = new Date(timestamp).getMonth() + 1;
      let Year = new Date(timestamp).getFullYear();
      let OurNewDateFormat = `${Day}/${Month}/${Year}`;
      return OurNewDateFormat;
    },
    handleSearch(){
      console.log(this.content)
    },
    clearSearch() {
      this.filter = null;
    },
    getNameUser(name) {              
        let user = this.listuser.find(x => x.userName == name);
          return user.fullName;
    },
    handleGetDonVi() {
      this.$store.dispatch("userStore/getAll").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listuser = res.data;
        } 
      });
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
        let promise = this.$store.dispatch("projectStore/getPagingParams", params)
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
    handleNewPost() {
      this.$router.push("/tao-project")
    },
    handleDetail(slug) {
      this.$router.push(`/${slug}/chi-tiet`)
    },
    handleDetailProject(slug) {

      let currentProject = JSON.stringify(slug)

      localStorage.setItem("currentProject", currentProject);

      this.$router.push(`/${slug}/danh-sach-yeu-cau-loi`)
    }
  },
};
</script>

<template>
  <Layout>
    <!--    <PageHeader/>-->
    <div class="row">
      <div class="col-12">
        <div class="card mb-2">
          <div class="card-body">
            <div class="row">
              <div class="col-md-4 col-12 d-flex align-items-center">
                <h4 class="font-size-18 fw-bold cs-title-page">Danh sách dự án</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light" class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
                </b-button>
                <b-button variant="primary" type="button" class="btn w-md btn-primary" @click="handleNewPost" size="sm">
                  <i class="mdi mdi-plus me-1"></i> Thêm dự án
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
                        <input v-model="filter" type="text" class="form-control" placeholder="Tìm kiếm ..."/>
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
                
                <div class="table-responsive">
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
                      tbody-tr-class="b-table-chucvu linkcolor"
                  >
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage - 1) * perPage) + 1 }}
                    </template>


                    <template v-slot:cell(name)="data">                  
                      <a style="cursor: pointer;" v-on:click="handleDetailProject(data.item.slug)">
                        <div>
                          <b style="font-size: 17px; color: black;"> {{ data.item.name }}</b>
                          <div class="ellips" style="color: dimgray;">{{ data.item.description }}</div>
                        </div>
                      </a>
                    </template>
                    <template v-slot:cell(createdAt)="data">                  
                          <p style="font-size: 15px; color: black;">{{ convertdate(data.item.createdAt) }}</p>
                    </template>  
                    <template v-slot:cell(createdBy)="data">                  
                          <p style="font-size: 15px; color: black;">{{getNameUser(data.item.createdBy) }}</p>
                    </template>  
                    <template v-slot:cell(process)="data">
                      <button type="button" size="sm" class="btn btn-detail btn-sm" data-toggle="tooltip"
                        data-placement="bottom" title="Chi tiết" v-on:click="handleDetail(data.item.slug)">
                        <i class="fas fa-eye "></i>
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
                        <!-- pagination -->
                        <b-pagination class="pagination-rounded" v-model="currentPage" :total-rows="totalRows"
                          :per-page="perPage" size="sm"></b-pagination>
                      </ul>
                    </div>
                  </div>
                </div>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style >
.table th>div {
    color: #000000;
    font-weight: 560;
    font-size: 16px;
}
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

.td-donVi {
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

.parent {
  width: 300px;
}

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
  -webkit-line-clamp: 1;
  -webkit-box-orient: vertical;
  overflow: hidden;
  text-overflow: ellipsis;
}
.linkcolor:hover
   {
    background-color: rgb(218, 235, 250);
}
</style>
<style lang="scss">
#my-strictly-unique-vue-upload-multiple-image {
  font-family: 'Avenir', Helvetica, Arial, sans-serif;
  -webkit-font-smoothing: antialiased;
  -moz-osx-font-smoothing: grayscale;
  text-align: center;
  color: #2c3e50;
}

</style>