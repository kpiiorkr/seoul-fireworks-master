# Unity 6.5 WebGL 배포 가이드

## 1. WebGL 타깃 전환
1. Unity 메뉴에서 `File > Build Profiles`(또는 Build Settings) 오픈
2. Platform을 `Web`(WebGL)로 선택 후 `Switch Platform`
3. Scene in Build에 메인 씬 추가 확인

## 2. Player Settings 권장값 (Unity 6.5)
- Company/Product Name: 배포용 명칭으로 정리
- Resolution and Presentation
  - Default Canvas Width/Height: 1920x1080 기준
  - Run In Background: Off
- WebGL
  - Compression Format: **Brotli**
  - Decompression Fallback: **On**
  - Data Caching: On
  - Memory Growth Mode: Geometric
  - Linker Target: WASM

## 3. 입력/해상도
- UI는 TextMeshProUGUI 사용 유지
- Canvas Scaler: Scale With Screen Size, Reference Resolution 1920x1080, Match 0.5
- 모바일 터치 입력은 버튼 OnClick 기반으로 동작

## 4. 원클릭 WebGL 빌드
- 메뉴: `Tools > Seoul Firework > Build WebGL`
- 출력 경로: `Build/WebGL`
- 생성 파일: `index.html`, `Build/*.wasm`, `Build/*.data`, `Build/*.js`

## 5. 정적 호스팅 배포
WebGL 빌드 산출물은 정적 파일이므로 아래에 그대로 업로드 가능:
- GitHub Pages
- Netlify
- Vercel
- itch.io(HTML)

## 6. 로컬 테스트
WebGL 산출물은 파일 직접 열기 대신 HTTP 서버로 확인:
```powershell
cd Build\WebGL
python -m http.server 8080
```
브라우저에서 `http://localhost:8080` 접속
