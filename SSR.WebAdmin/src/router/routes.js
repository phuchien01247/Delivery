import store from '@/state/store'

export default [{
        path: '/',
        meta: {
            authRequired: true
        },
        name: 'home',
        component: () => import('../pages/dashboard/index'),
    },
    {
        path: '/dang-nhap',
        name: 'login',
        component: () => import('../pages/auth/auth'),
        meta: {
            beforeResolve(routeTo, routeFrom, next) {
                // If the user is already logged in
                const loggeduser = localStorage.getItem('user-token');
                if (loggeduser) {
                    // Redirect to the home page instead
                    window.location.href = '/'
                } else {
                    // Continue to the login page
                    next()
                }
            },
        },
    },
    {
        path: '/register',
        name: 'register',
        component: () => import('./views/account/register'),
        meta: {
            beforeResolve(routeTo, routeFrom, next) {
                // If the user is already logged in
                if (store.getters['auth/loggedIn']) {
                    // Redirect to the home page instead
                    next({
                        name: 'home'
                    })
                } else {
                    // Continue to the login page
                    next()
                }
            },
        },
    },
    {
        path: '/pages/404',
        name: 'Page-404',
        component: () => import('./views/pages/error-404'),
        meta: {
            authRequired: true,
        }
    },
    {
        path: '/pages/500',
        name: 'Page-500',
        component: () => import('./views/pages/error-500'),
        meta: {
            authRequired: true,
        }
    },
    {
        path: '/group',
        name: 'Group',
        component: () => import('../pages/group/group'),
        meta: {
            authRequired: true,
        }
    },
    {
        path: "/nhom-quyen",
        name: "NhomQuyen",
        meta: {
            authRequired: true,
        },
        component: () => import("../pages/module"),
    },
    {
        path: "/nhom-quyen/action/:id?",
        name: "Hành động",
        // meta: {},
        component: () => import("../pages/module/action"),
    },
    {
        path: "/menu",
        name: "Menu",
         meta: {          authRequired: true, },
        component: () => import("../pages/menu"),
    },
    {
        path: "/vai-tro",
        name: "Quyền",
        meta: {can: 'viewpage-roles'},
        component: () => import("../pages/role"),
    },
    {
        path: "/vai-tro/thiet-lap-quyen/:id?",
        name: "Vai trò",
        meta: {can: 'viewpage-roles' },
        component: () => import("../pages/role/addPermissions01"),
    },
    {
        path: "/add-permissions",
        name: "Test ",
        component: () => import("../pages/role/addPermissions"),
    },
    {
        path: "/tai-khoan",
        name: "Tài khoản",
        meta: {
            can: 'viewpage-user',
        },
        component: () => import("../pages/user"),
    },
    {
        path: "/tai-khoan/doi-mat-khau",
        name: "Đổi mật khẩu",
        meta: {can: 'viewpage-user', },
        component: () => import("../pages/user/ChangePass"),
    },
    {
        path: "/don-vi",
        name: "Đơn vị",
        meta: { },
        component: () => import("../pages/donVi"),
    },
    {
        path: "/loggings",
        name: "Loggings",
        meta: {},
        component: () => import("../pages/loggings"),
    },
    {
        path: "/thong-tin-ca-nhan",
        name: "Thông tin cá nhân",
        // meta: {},
        component: () => import("../pages/auth/profile"),
    },
   
    {
        path: "/huong-dan-xu-ly",
        name: "Hướng dẫn xử lý",
        // meta: {},
        component: () => import("../pages/danhMuc"),
    },
    {
        path: "/danh-sach-yeu-cau-loi",
        name: "Yêu cầu lỗi",
        // meta: {},
        component: () => import("../pages/yeucauloi/index"),
    },
    {
        path: "/:slug/yeu-cau-loi/detail/:id",
        name: "Yêu cầu lỗi",
        // meta: {},
        component: () => import("../pages/yeucauloi/detail"),
    },

    {
        path: "/du-an",
        name: "du-an",
        // meta: {},
        component: () => import("../pages/DuAn/index1.vue"),
    },
    {
        path: "/du-an/:?",
        name: "project",
        // meta: {},
        component: () => import("../pages/DuAn/create"),
    },
    {
        path: "/tao-project",
        name: "danh-sach-yeu-cau-loi",
        // meta: {},
        component: () => import("../pages/DuAn/create.vue"),
    },
    {

        path: "/buoc-thuc-hien",
        name: "buoc-thuc-hien",
        // meta: {},
        component: () => import("../pages/step"),
    },
    {
        path: "/du-an/chi-tiet/:id",
        name: "Vai trò",
        meta: {can: 'viewpage-roles' },
        component: () => import("../pages/DuAn/detail"),
    },
    {
        path: "/:slug/danh-sach-yeu-cau-loi",
        name: "Du an",
        component: () => import("../pages/yeucauloi/index"),
    },
    {
        // https://localhost:5001/api/v1/Label/get-with-projid?id=6407e6bcfa0a818e221469c3
        path: "/:slug/nhan-du-an",
        name: "Nhan du an",
        component: () => import("../pages/label/indexinproject"),
    },
    {
        path: "/:slug/yeu-cau-loi",
        name: "Them yeu cau loi",
        
        component: () => import("../pages/yeucauloi/create"),
    },
    {
        path: "/:slug/yeu-cau-loi/:id",
        name: "Them yeu cau loi",
        
        component: () => import("../pages/yeucauloi/create"),
    },
    {
        path: "/:slug/thong-tin",
        name: "Thong tin",
        
        component: () => import("../pages/DuAn/detail"),
    },
    {
        path: "/thong-ke",
        name: "ThongKe",
        
        component: () => import("../pages/thongke"),
    },
    {
        path: "/:slug/buoc-thuc-hien",
        name: "Buoc Thuc Hien",
        
        component: () => import("../pages/step/index"),
    },
    {
        path: "/:slug/thong-ke",
        name: "Nhãn",
        // meta: {},
        component: () => import("../pages/thongke/index"),
    },
    {
        path: "/:slug/chi-tiet",
        name: "Chi tiết dự án",
        // meta: {},
        component: () => import("../pages/chitiet"),
    },
    {
        path: "/Label",
        name: "Nhãn",
        // meta: {},
        component: () => import("../pages/label/index"), 
    },
    {
        path: "/loi-can-xu-ly",
        name: "Lỗi Trong Ngày",
        // meta: {},
        component: () => import("../pages/yeucauloi/indexloitrongngay"), 
    },
    {
        path: "/yeu-cau-loi/detailloi/:id",
        name: "Lỗi Trong Ngày",
        // meta: {},
        component: () => import("../pages/yeucauloi/detailloi"), 
    },
    {
        path: "/loi-trong-ngay",
        name: "Lỗi Trong Ngày",
        // meta: {},
        component: () => import("../pages/yeucauloi/indexloingay"), 
    },
    {
        path: "/yeu-cau-loi/detailloi/:id",
        name: "Lỗi Trong Ngày",
        // meta: {},
        component: () => import("../pages/yeucauloi/detailloi"), 
    },
    {
        path: "/loi-da-giai-quyet",
        name: "Lỗi đã giải quyết",
        // meta: {},
        component: () => import("../pages/yeucauloi/indexloidagiaiquyet"), 
    },
]
