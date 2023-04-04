
const baseJson = () => {
    return {
        id: null,
        title: null,
        summary: null,
        content: "<p> Nội dung </p>",
        thumbnail: null,
        published: false,
        publishedAt: null,
        slug: null,
        code: null,
        category: null,
        tags: null,
        files: null,
        status: null,
        uploadFiles: null,
        createdAt: null,
        modifiedAt: null,
        createdBy: null,
        modifiedBy: null,
        isDeleted: false
    }
}

export const postModel = {
  baseJson
}
