<script>
import Layout from "../../layouts/main";

export default {
  components: { Layout },
  data() {
    return {

    };
  },
};
</script>

<template>
  <Layout>
    <div class="row">
      <div class="col-12">
        <!-- Left sidebar -->
        <div class="email-leftbar card">
          <router-link

              to="/hop-thu/soan-thu"
              class="btn btn-danger rounded btn-custom btn-block"
          >Soạn thư</router-link
          >

          <div class="mail-list mt-4 cs-email">

            <router-link
                tag="a"
                class-active="active"
                class="p-2 item-link"
                to="/thu-den"
                exact
            >
              Thư đến
              <span class="ms-1">(18)</span>
            </router-link>
            <router-link
                tag="a"
                class-active="active"
                class="p-2 item-link"
                to="/thu-da-gui"
                exact
            >
              Thư đã gửi
              <span class="ms-1">(18)</span>
            </router-link>
            <router-link
                tag="a"
                class-active="active"
                class="p-2 item-link"
                to="/thu-rac"
                exact
            >
              Thư rác
              <span class="ms-1">(18)</span>
            </router-link>
          </div>
        </div>
        <!-- End Left sidebar -->

        <!-- Right Sidebar -->
        <div class="email-rightbar mb-3">
          <slot></slot>
        </div>
        <!-- End Right Sidebar -->
      </div>
    </div>
  </Layout>
</template>

<style>
.cs-email .item-link{
  color: #045f90 !important;
  font-weight: 500;
}

.cs-email .router-link-active{
  color: #00568c !important;
  position: relative;
}
.cs-email .router-link-active::before{
  content: "";
  display: block;
  height: 3px;
  width: 90%;
  background-color: #f5214b;
  border-radius: 50px;
  position: absolute;
  left: 4%;
  bottom: 0px;
}
</style>