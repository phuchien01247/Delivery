<template>
    <div class="vertical-menu">
        <div class="h-100" data-simplebar="init">
            <div class="simplebar-wrapper" style="margin: 0px;">
                <div class="simplebar-height-auto-observer-wrapper">
                    <div class="simplebar-height-auto-observer"></div>
                </div>
                <div class="simplebar-mask">
                    <div class="simplebar-offset" style="right: 0px; bottom: 0px;">
                        <div class="simplebar-content-wrapper" tabindex="0" role="region" aria-label="scrollable content"
                            style="height: 100%; overflow: hidden;">
                            <div class="simplebar-content" style="padding: 0px;">
                                <div id="sidebar-menu" class="p-0">
                                    <ul id="side-menu" class="metismenu list-unstyled">
                                        <li>
                                            <!-- <a :href="`/${slug}/chi-tiet`" class="side-nav-link router-link-active"> -->
                                            <router-link :to="`/${slug}/chi-tiet`" class="side-nav-link router-link-active">
                                                <img style="height: 25px;width: 25px;margin-top: -10px;"
                                                    src="@/assets/project.jpg"
                                                    class="flex-shrink-0 me-3 rounded mx-auto d-blocks avatar-sm">
                                               
                                                <span >
                                                <p style="margin-bottom: -5px;max-width: 150px" class="d-inline-block text-truncate align-items-center">{{ model.name }}</p>    
                                                </span>


                                            </router-link>

                                            <!-- </a> -->
                                        </li>
                                        <li>

                                            <router-link :to="`/${slug}/danh-sach-yeu-cau-loi`"
                                                class="side-nav-link router-link-active">
                                                <i class="mdi mdi-alert-outline"></i>
                                                <span>Danh sách yêu cầu</span>
                                                <span class="badge rounded-pill bg-danger float">{{ issuedataopen.length
                                                }}</span>
                                            </router-link>

                                        </li>
                                        <!-- <li>
                                        <router-link
                                                    :to="`/${slug}/chi-tiet`"
                                                    
                                                    class="side-nav-link router-link-active"
                                                >
                                                <i class="bx ti-agenda"></i>
                                                <span>Phân quyền</span>
                                         </router-link>
                                        </li> -->
                                        <li>
                                            <router-link :to='`/${slug}/nhan-du-an`'
                                                class="side-nav-link router-link-active">
                                                <i class="fas fa-tags"></i>
                                                <span>Nhãn dự án</span>
                                            </router-link>
                                        </li>
                                        <li>
                                            <router-link :to='`/${slug}/thong-ke`' class="side-nav-link router-link-active">
                                                <i class="mdi mdi-file-chart"></i>
                                                <span>Thống kê</span>
                                            </router-link>
                                        </li>
                                       <!--  <li>
                                            <router-link :to='`/${slug}/buoc-thuc-hien`'
                                                class="side-nav-link router-link-active">
                                                <i class="mdi mdi-debug-step-into"></i>
                                                <span>Bước thực hiện</span>
                                            </router-link>
                                        </li> -->
                                        <li >
                                            <div v-if="isActive == true">
                                            <router-link  :to='`/${slug}/thong-tin`'
                                                class="side-nav-link router-link-active">
                                                <i class="mdi mdi-cog"></i>
                                                <span>Cài đặt dự án</span>
                                            </router-link>
                                        </div>
                                        </li>
                                    </ul>
                                </div>
                            </div>
                        </div>
                    </div>
                    <div class="simplebar-placeholder" style="width: auto; height: 199px;"></div>
                </div>
                <div class="simplebar-track simplebar-horizontal" style="visibility: hidden;">
                    <div class="simplebar-scrollbar" style="width: 0px; display: none;"></div>
                </div>
                <div class="simplebar-track simplebar-vertical" style="visibility: hidden;">
                    <div class="simplebar-scrollbar" style="height: 0px; display: none;"></div>
                </div>
            </div>
        </div>
    </div>
</template>
    
<script>
import { projectModel } from "@/models/projectModel";

export default {
    data() {
        
        return {
            slug: null,
            model: projectModel.baseJson(),
            data: [],
            issuedata: "",
            issuedataopen: "",
            issuedataclose: "",
            isActive: false,
        }
    },
    created() {
        let currentProjectLocal = localStorage.getItem('currentProject');
        this.slug = JSON.parse(currentProjectLocal)
        if (!this.slug) {
            this.$router.push(`/${this.slug}/du-an`)
        }
        if (this.$route.params.slug) {
            this.getBySlug(this.$route.params.slug);
        } else {
            this.model = projectModel.baseJson();
        }
        this.GetList();
    },
    methods: {
        async getBySlug(slug) {
            let currentUser = localStorage.getItem('auth-user');
            this.user = JSON.parse(currentUser);
            await this.$store.dispatch("projectStore/getBySlug", slug).then((res) => {
                if (res.resultCode === 'SUCCESS') {
                    this.model = res.data;
                    if (this.model.createdBy == this.user.userName) {
                       
                        this.isActive = true;
                    }
                    else {
                        this.isActive = false;
                    }
                }
            });

        },
        async GetList() {
            let currentProjectLocal = localStorage.getItem('currentProject');
            this.slug = JSON.parse(currentProjectLocal);
            //lấy ds yêu cầu
            await this.$store.dispatch("yeucauloiStore/get").then((res) => {
                this.listissue = res.data;
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
                    //console.log(this.idproject);
                    //tìm có idproject giống
                    const issue = this.listissue.find(p => p.projectId == this.idproject);

                    if (issue) {
                        this.$store.dispatch("yeucauloiStore/getwithprojid", this.idproject).then((res) => {
                            this.issuedata = res.data;
                        })
                        this.$store.dispatch("yeucauloiStore/getopenwithprojid", this.idproject).then((res) => {
                            this.issuedataopen = res.data;
                        })
                        this.$store.dispatch("yeucauloiStore/getclosewithprojid", this.idproject).then((res) => {
                            this.issuedataclose = res.data;
                        })
                    }
                    else {
                        this.issuedata = [];
                        this.issuedataclose = [];
                        this.issuedataopen = []
                    }
                }
            });

        },
    }
}

</script>
<style>
.text-trun {

    display: -webkit-box;
    -webkit-line-clamp: 2;
    -webkit-box-orient: vertical;
    overflow: hidden;
}
</style>