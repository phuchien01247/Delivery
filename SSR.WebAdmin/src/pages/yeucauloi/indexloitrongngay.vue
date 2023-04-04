<script>
import Layout from "../../layouts/main";
import appConfig from "@/app.config";
import { yeucauloiModel } from "@/models/yeucauloiModel";
import { required } from "vuelidate/lib/validators";
import { notifyModel } from "@/models/notifyModel";
import { pagingModel } from "@/models/pagingModel";
import { CONSTANTS } from "@/helpers/constants";
import { donViModel } from "@/models/donViModel";
export default {
  page: {
    title: "Danh sách lỗi cần xử lý",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: { Layout },
  data() {
    return {
      title: "Danh sách lỗi cần xử lý",
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
      sortBy: "age",
      sortDesc: false,
      model: yeucauloiModel.baseJson(),
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
          thStyle: { width: '40px', minWidth: '40px' }
        },
        {
          key: "name",
          label: "Yêu cầu",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '600px' },
        },
        {
          key: "dueDate",
          label: "Ngày đáo hạn",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '600px' },
        },
        {
          key: "issue",
          label: "Phân công",
          sortable: true,
          thClass: 'hidden-sortable',
          thStyle: { width: '600px' },
        },
        {
          key: 'process',
          label: 'Xử lý',
          class: 'td-xuly btn-process',
          thClass: 'hidden-sortable',
          // sortable: false,
          thStyle: { width: '100px', minWidth: '100px' },
        }
      ],
    };
  },

  async created() {
    if (this.$route.params.id) {
      this.getById(this.$route.params.id);
    } else {
      this.model = yeucauloiModel.baseJson();
    }
    if (this.$route.params.slug) {
      this.getBySlug(this.$route.params.slug);
    } else {
      this.model = yeucauloiModel.baseJson();
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
    async handleUpdate(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = yeucauloiModel.toJson(res.data);
          let currentProjectLocal = localStorage.getItem('currentProject');
          this.slug = JSON.parse(currentProjectLocal);
          this.$router.push(`/${this.slug}/yeu-cau-loi/${id}`);

        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    getNameUser(name) {
      let currentUser = localStorage.getItem('auth-user');
      this.user = JSON.parse(currentUser);
      if (name == this.user.userName) {
        return this.isActive = true;
      }
      else {
        return this.isActive = false;
      }
    },
    getNameDonViWithId(id) {
      let donVi = this.listDonvi.find(x => x.id == id);
      if (donVi)
        return donVi.name;
      return null;
    },
    handleGetDonVi() {
      this.$store.dispatch("donViStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listDonvi = res.data;
        }
      });
    },
    converttime(date) {
      this.lastUpdated = date;
      const lastUpdatedDate = new Date(this.lastUpdated);
      const currentTime = new Date();
      const timeDifferenceInMilliseconds = currentTime.getTime() - lastUpdatedDate.getTime();

      // Tính toán khoảng thời gian giữa thời điểm hiện tại và thời điểm cập nhật lần cuối
      // tính số phút/giờ/ngày/tuần trước
      const timeDifferenceInMinutes = Math.floor(timeDifferenceInMilliseconds / (1000 * 60));
      const timeDifferenceInHours = Math.floor(timeDifferenceInMilliseconds / (1000 * 60 * 60));
      const timeDifferenceInDays = Math.floor(timeDifferenceInMilliseconds / (1000 * 60 * 60 * 24));
      const timeDifferenceInWeeks = Math.floor(timeDifferenceInMilliseconds / (1000 * 60 * 60 * 24 * 7));

      // Chuyển đổi khoảng thời gian thành chuỗi "cách đây ... phút/giờ/ngày/tuần trước"
      if (timeDifferenceInMinutes < 60) {
        this.lastUpdatedText = `cách đây ${timeDifferenceInMinutes} phút trước`;
      } else if (timeDifferenceInHours < 24) {
        this.lastUpdatedText = `cách đây ${timeDifferenceInHours} giờ trước`;
      } else if (timeDifferenceInDays < 7) {
        this.lastUpdatedText = `cách đây ${timeDifferenceInDays} ngày trước`;
      } else {
        this.lastUpdatedText = `cách đây ${timeDifferenceInWeeks} tuần trước`;
      }
      return this.lastUpdatedText;
    },
    convertdate(date) {
      let newsApiDate = date; // got from the Api
      let timestamp = new Date(newsApiDate).getTime();
      let Day = new Date(timestamp).getDate();
      let Month = new Date(timestamp).getMonth() + 1;
      let Year = new Date(timestamp).getFullYear();
      let OurNewDateFormat = `${Day}/${Month}/${Year}`;
      return OurNewDateFormat;
    },
    async handleRedirectToDetail(id) {
      await this.$store.dispatch("yeucauloiStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = yeucauloiModel.toJson(res.data);

          this.$router.push(`/yeu-cau-loi/detailloi/${id}`);
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
    },
    handleSearch() {
      console.log(this.content)
    },
    clearSearch() {
      this.filter = null;
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
        let promise = this.$store.dispatch("yeucauloiStore/getPagingParamsLoiTrongNgay", params)
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
                <h4 class="font-size-18 fw-bold cs-title-page">Danh sách lỗi cần xử lý</h4>
              </div>
              <div class="col-md-8 col-12 text-end">
                <b-button v-b-toggle.collapseSearch variant="light" class="btn w-md btn-primary-outline me-2" size="sm">
                  <i class="fas fa-caret-down align-middle me-2"></i>
                  Tìm kiếm
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
                  <b-table class="datatables custom-table" :items="myProvider" :fields="fields" responsive="sm"
                    :per-page="perPage" :current-page="currentPage" :sort-by.sync="sortBy" :sort-desc.sync="sortDesc"
                    :filter="filter" :filter-included-fields="filterOn" ref="tblList" primary-key="id" :busy.sync="isBusy"
                    tbody-tr-class="b-table-chucvu linkcolor">
                    <template v-slot:cell(STT)="data">
                      {{ data.index + ((currentPage - 1) * perPage) + 1 }}
                    </template>
                    <template v-slot:cell(name)="data">
                      <a style="cursor: pointer;" v-on:click="handleRedirectToDetail(data.item.id)">
                        <div style="font-size: 15px;">
                          <strong style="cursor: pointer;"> {{ data.item.title }}</strong>
                          <br>
                          <span class="badge rounded-pill"
                            style="background-color:rgb(161 229 224 / 29%); color: rgb(2 164 153); font-size: 13px; margin-right: 5px">
                            {{ getNameDonViWithId(data.item.donVi) }}
                          </span>
                          <span class="badge rounded-pill" v-for="(value, index) in data.item.label" :key="index"
                            v-bind:style="{ background: value.color }">
                            {{ value.name }}
                          </span>
                        </div>
                      </a>
                    </template>
                    <template v-slot:cell(dueDate)="data">
                      {{ convertdate(data.item.dueDate) }}
                    </template>
                    <template v-slot:cell(issue)="data">
                      <div style="font-size: 17px; ">
                        <span class="badge rounded-pill"
                          style="background-color:rgb(161 229 224 / 29%); color: rgb(2 164 153);"
                          v-for="(value, index) in data.item.group" :key="index">
                          {{ value.name }}
                        </span>
                        <span class="badge rounded-pill"
                          style="background-color:rgb(161 229 224 / 29%); color: rgb(2 164 153);"
                          v-for="(value, index) in data.item.user" :key="index">
                          {{ value.fullName }}
                        </span>
                      </div>
                    </template>
                    <template v-slot:cell(process)="data">
                      <button type="button" size="sm" class="btn btn-detail btn-sm" data-toggle="tooltip"
                        data-placement="bottom" title="Chi tiết" v-on:click="handleRedirectToDetail(data.item.id)">
                        <i class="fas fa-eye "></i>
                      </button>
                      <button v-if="getNameUser(data.item.createdBy)" type="button" size="sm" class="btn btn-edit btn-sm"
                        v-on:click="handleUpdate(data.item.id)">
                        <i class="fas fa-pencil-alt"></i>
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

.linkcolor:hover {
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