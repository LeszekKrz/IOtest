import { VideoFromPlaylistDto } from "./video-from-playlist-dto";

export interface PlaylistVideosDto {
    name: string;
    visibility: string;
    videos: VideoFromPlaylistDto[];
  }
  