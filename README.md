### 4.22

- API 통신을 위한 Controller 및 HostBuilder 추가
    - /Controller/MatchingController
        - 현재 호출 시, 바로 Grpc 통신으로 연결.
        추후, 대기열 & 매칭 로직 추가 필요
        - 통신 응답 반환(IP & Port)
- Grpc를 통해 다른 서버와 통신하는 부분 추가
    - proto 추가
        - /Protos/matching
    - Grpc.Tools을 통해 proto 컴파일
    - MatchingController에 임시 구현