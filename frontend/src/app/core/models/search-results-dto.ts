import {UserDTO} from './user-dto'
import { VideoMetadataDto } from './video-metadata-dto'

export interface SearchResultsDTO {
    videos: VideoMetadataDto [],
    users: UserDTO []
}