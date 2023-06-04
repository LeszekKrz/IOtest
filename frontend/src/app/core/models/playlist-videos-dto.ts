import { VideoMetadataDto } from "./video-metadata-dto";

export interface PlaylistVideosDto {
    name: string;
    visibility: string;
    videos: VideoMetadataDto[];
    authorId: string;
    authorNickname: string;
  }
  