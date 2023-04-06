export interface VideoMetadataDto {
    id: string
    title: string
    description: string
    thumbnail: string
    authorId: string
    authorNickname: string
    viewCount: number
    tags: string[]
    visibility: string
    processingProgress: string
    uploadDate: Date
    editDate: Date
    duration: string
}
  