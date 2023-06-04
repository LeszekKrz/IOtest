import {UserDTO} from './user-dto'
import { VideoMetadataDto } from './video-metadata-dto'
import { PlaylistBaseDto } from './playlist-base-dto'

export interface SearchResultsDTO {
    videos: VideoMetadataDto [],
    users: UserDTO [],
    playlists: PlaylistBaseDto []
}