<script>
import appConfig from "@/app.config";

/**
 * Recover password-1 component
 */
export default {
  page: {
    title: "Recover password-1",
    meta: [{ name: "description", content: appConfig.description }],
  },
};
</script>
<template>
  <div>
    <div class="home-btn d-none d-sm-block">
      <router-link to="/" class="text-dark">
        <i class="fas fa-home h2"></i>
      </router-link>
    </div>
    <div class="account-pages my-5 pt-5">
      <div class="container">
        <div class="row justify-content-center">
          <div class="col-md-8 col-lg-6 col-xl-4">
            <div class="card overflow-hidden">
              <div class="bg-primary">
                <div class="text-primary text-center p-4">
                  <h5 class="text-white font-size-20 p-2">Reset Password</h5>
                  <router-link to="/" class="logo logo-admin">
                    <img
                      src="@/assets/images/logo-sm.png"
                      height="24"
                      alt="logo"
                    />
                  </router-link>
                </div>
              </div>

              <div class="card-body p-4">
                <div class="p-3">
                  <div class="alert alert-success mt-5" role="alert">
                    Enter your Email and instructions will be sent to you!
                  </div>

                  <form class="form-horizontal mt-4">
                    <div class="mb-3">
                      <label class="form-label" for="useremail">Email</label>
                      <input
                        type="email"
                        class="form-control"
                        id="useremail"
                        placeholder="Enter email"
                      />
                    </div>

                    <div class="form-group row mb-0">
                      <div class="col-12 text-end">
                        <button class="btn btn-primary w-md" type="submit">
                          Reset
                        </button>
                      </div>
                    </div>
                  </form>
                </div>
              </div>
            </div>

            <div class="mt-5 text-center">
              <p>
                Remember It ?
                <router-link to="/pages/login-1" class="fw-medium text-primary"
                  >Sign In here</router-link
                >
              </p>
              <p class="mb-0">
                ©
                {{ new Date().getFullYear() }}
                Veltrix. Crafted with
                <i class="mdi mdi-heart text-danger"></i> by Themesbrand
              </p>
            </div>
          </div>
        </div>
      </div>
    </div>
  </div>
</template>
