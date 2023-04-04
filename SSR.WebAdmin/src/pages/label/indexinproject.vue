<script>
import Layout from "../../layouts/main1";
import PageHeader from "@/components/page-header";
import { required } from "vuelidate/lib/validators";
import appConfig from "@/app.config";
import { notifyModel } from "@/models/notifyModel";
import { menuModel } from "@/models/menuModel";
import Treeselect from "@riophae/vue-treeselect";
import { linhVucModel } from "@/models/linhVucModel";
import { labelinprojectModel } from "@/models/labelinprojectModel";
import { knowledgeModel } from "@/models/knowledgeModel";
import Multiselect from "vue-multiselect";

export default {
  page: {
    title: "Quản lý nhãn",
    meta: [{ name: "description", content: appConfig.description }],
  },
  components: { Layout, Multiselect, Treeselect },
  data() {
    return {
      // data: [],
      title: "Quản lý nhãn",
      listKnowledge: [],
      items: [
        {
          text: "Nhãn",
          active: true,
        },
        {
          text: "Danh sách",
          active: true,
        }
      ],
      showModal: false,
      showDeleteModal: false,
      submitted: false,
      model: labelinprojectModel.baseJson(),
      listParent: [],
      treeView: [],
      itemEvents: {
        mouseover: function () {
          console.log("mouseover");
        },
        contextmenu: function () {
          console.log(arguments[2]);
          arguments[2].preventDefault();
          console.log("contextmenu");
        },
      },
      itemFilter: {
        code: null,
        name: null,
      }
    };
  },

  validations: {
    model: {
      name: { required },
      color: { required },
    },
  },
  created() {
    this.GetTreeList();
    this.getListKnowledge();
    this.getdata();
    this.handleCreate();
  },
  methods: {
    addDonViToModel(node, instanceId) {
      if (node.id) {
        this.model.parentId = node.id;
      }
    },
    async getdata() {
      await this.$store.dispatch("labelStore/get").then((res) => {
        this.listdata = res.data || [];
      })
    },
    fnGetList() {
      this.$refs.tblList?.refresh()
    },
    clearSearch() {
      this.itemFilter.code = null;
      this.itemFilter.name = null;
    },
    normalizer(node) {
      if (node.children == null || node.children == 'null') {
        delete node.children;
      }
    },
    async GetTreeList() {
      let currentProjectLocal = localStorage.getItem('currentProject');
      this.slug = JSON.parse(currentProjectLocal);
      //lấy ds nhãn
      await this.$store.dispatch("labelStore/get").then((res) => {
        this.listlabel = res.data;
      })

      //lấy ds dự án
      this.$store.dispatch("projectStore/getBySlug", this.slug).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listProject = res.data;
          this.nameproject = JSON.parse(currentProjectLocal); //lưu tên dự án đang mở

      //tìm tên dự án đang mở trong listproject để lấy id

      if (this.listProject) {
        this.idproject = this.listProject.id;     //chứa idproject đang mở
      }

      //tìm label có idproject giống
      const label = this.listlabel.find(p => p.idProject == this.idproject);
      
      if (label) { 
        this.$store.dispatch("labelStore/getTreewithprojid", this.idproject).then((res) => {
        this.treeView = res.data;
        //console.log(this.labeldata)
      })}
      else{
        this.$store.dispatch("labelStore/getTree", this.idproject).then((res) => {
        this.treeView = res.data;
      })}
        }
      });
    },
    handleCreate() {
      let currentProjectLocal = localStorage.getItem('currentProject');

      this.$store.dispatch("projectStore/get").then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.listProject = res.data; //lấy hết cái list
          this.nameproject = JSON.parse(currentProjectLocal);  //test6

          const project = this.listProject.find(p => p.slug === this.nameproject)
          if (project) {
            this.idproject = project.id;
            console.log(this.idproject);
          }
          else {
            this.idproject = null;
          }
          this.model.idProject = this.idproject;


        }
        this.listProject = [];
      });
    },
    async getListKnowledge() {
      await this.$store.dispatch("knowledgeStore/get").then((res) => {
        this.listKnowledge = res.data || [];
      })
    },
    async itemClick(node) {
      this.handleDetail(node.model.id)
    },

    async handleDetail(id) {
      await this.$store.dispatch("labelStore/getById", id).then((res) => {
        if (res.resultCode === 'SUCCESS') {
          this.model = labelinprojectModel.fromJson(res.data);
          this.showDetail = true;
        } else {
          this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res));
        }
      });
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
          await this.$store.dispatch("labelStore/update", this.model).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.GetTreeList();
              this.getListKnowledge();
              this.handleCreate();
              this.model = labelinprojectModel.baseJson()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        } else {
          // Create model
          await this.$store.dispatch("labelStore/create", labelinprojectModel.toJson(this.model)).then((res) => {
            if (res.resultCode === 'SUCCESS') {
              this.showModal = false;
              this.GetTreeList();
              this.getListKnowledge();
              this.handleCreate();
              this.model = labelinprojectModel.baseJson()
            }
            this.$store.dispatch("snackBarStore/addNotify", notifyModel.addMessage(res))
          });
        }
        loader.hide();
      }
      this.submitted = false;
    },
    async handleDelete() {
      if (this.model.id != 0 && this.model.id != null && this.model.id) {
        await this.$store.dispatch("labelStore/delete", this.model.id).then((res) => {
          if (res.resultCode === 'SUCCESS') {
            this.showDeleteModal = false;
            this.GetTreeList();
            this.model = labelinprojectModel.baseJson();
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

    handleResetForm() {
      this.model = menuModel.baseJson()
    },
  },
  computed: {},
  watch: {
    showModal(status) {
      if (status == false) this.model = menuModel.baseJson();
    },
    model() {
      return this.model;
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
                <h4 class="font-size-18 fw-bold text-dark">Quản lý nhãn</h4>
              </div>
            </div>
          </div>
        </div>
      </div>
    </div>
    <div class="row flex-md-row flex-column-reverse">
      <div class="col-md-6 col-12">
        <div class="card">
          <div class="card-body">
            <v-jstree :text-color="model.color" :data="treeView" text-field-name="label" @item-click="itemClick" :item-events="itemEvents">
            </v-jstree>
          </div>
        </div>
      </div>
      <div class="col-md-6 col-12">
        <div class="card">
          <div class="card-body">
            <form @submit.prevent="handleSubmit" ref="formContainer">
              <div class="row">
                <div class="col-12">
                  <div class="mb-3">
                    <label class="text-left">Tên nhãn</label>
                    <span style="color: red">&nbsp;*</span>
                    <input id="name" v-model.trim="model.name" type="text" class="form-control" @change="handleCreate()"
                      placeholder="Nhập tên nhãn" :class="{
                        'is-invalid':
                          submitted && $v.model.name.$error,
                      }" />
                    <div v-if="submitted && !$v.model.name.required" class="invalid-feedback">
                      Tên nhãn không được để trống.
                    </div>
                  </div>

                  <div class="mb-3">
                    <label class="text-left">Chọn hướng dẫn</label>
                    <multiselect v-model="model.knowledge" :options="listKnowledge" track-by="id" label="name"
                      placeholder="Chọn hướng dẫn" deselect-label="Nhấn để xoá" selectLabel="Nhấn enter để chọn"
                      selectedLabel="Đã chọn" :multiple="true"></multiselect>
                    <multiselect v-model="selected" :options="options" :searchable="false" :close-on-select="false"
                      :allow-empty="false" placeholder="Select one" label="name" track-by="name">
                    </multiselect>
                  </div>

                  <div class="mb-3">
                    <label class="text-left">Lỗi cha</label>
                    <treeselect v-on:select="addDonViToModel" :normalizer="normalizer" :options="treeView"
                      :value="model.parentId" :searchable="true" :show-count="true" :default-expand-level="1"
                      placeholder="Chọn lỗi cha">
                      <label slot="option-label"
                        slot-scope="{ node, shouldShowCount, count, labelClassName, countClassName }"
                        :class="labelClassName">
                        {{ node.label }}
                        <span v-if="shouldShowCount" :class="countClassName">({{ count }})</span>
                      </label>
                    </treeselect>
                  </div>


                  <div class="row">
                    <div class="col-6">
                      <label class="text-left">Chọn màu nhãn</label>
                      <span style="color: red">&nbsp;*</span>
                      <br>
                      <input type="hidden" v-model="model.color" />
                      <input id="color" v-model="model.color" type="color" min="0" oninput="validity.valid||(value='');"
                        class="choosecolor" :class="{
                          'is-invalid':
                            submitted && $v.model.color.$error,
                        }" />
                      <div v-if="submitted && !$v.model.color.required" class="invalid-feedback">
                        Màu không được để trống.
                      </div>
                    </div>

                    
                    <div class="col-6">
                      <input id="name" v-model="model.idProject" type="text" class="form-control" hidden />
                    </div>
                  </div>



                </div>
              </div>
              <div class="text-end pt-2 mt-3">
                <b-button size="sm" v-if="model.id" type="button" variant="warning" class="ms-1 w-md"
                  v-on:click="handleResetForm"> Đặt lại
                </b-button>
                <b-button size="sm" v-if="model.id" type="button" variant="danger" class="ms-1 w-md"
                  v-on:click="handleShowDeleteModal(model.id)"> Xóa
                </b-button>
                <b-button type="submit" size="sm" variant="primary" class="ms-1 w-md">Lưu
                </b-button>
              </div>
            </form>
          </div>
          <b-modal v-model="showDeleteModal" centered title="Xóa dữ liệu" title-class="font-18" no-close-on-backdrop>
            <p>
              Dữ liệu xóa sẽ không được phục hồi !
            </p>
            <template #modal-footer>
              <button v-b-modal.modal-close_visit class="btn btn-outline-info m-1" v-on:click="showDeleteModal = false">
                Đóng
              </button>
              <button v-b-modal.modal-close_visit class="btn btn-danger btn m-1" v-on:click="handleDelete">
                Xóa
              </button>
            </template>
          </b-modal>
        </div>
      </div>
    </div>
  </Layout>
</template>
<style>
.tree-anchor span {
  padding: 2px;
  border-radius: 5px;
  color: black;
}
</style>
