# seoul-fireworks-master

## 1. 유니티 세팅방법
1. Unity Hub에서 **Unity 6.5** 에디터를 설치하고 새 2D(URP) 프로젝트를 연다.
2. 프로젝트에 `Assets/` 폴더(스크립트/SVG 포함)를 그대로 반영한다.
3. Package Manager에서 **TextMeshPro**와 **Vector Graphics** 패키지가 설치되어 있는지 확인한다.
4. `File > Build Profiles`(또는 Build Settings)에서 WebGL 플랫폼으로 전환한다.
5. 상단 메뉴 `Tools > Seoul Firework > Apply WebGL Player Defaults`를 실행해 WebGL 기본값(Brotli, Decompression Fallback, Data Caching)을 적용한다.
6. 메인 씬에 아래 오브젝트를 연결한다.
   - `GameManager` (FireworkData 5개, ComboRule 3개 할당)
   - `UIManager` (TMP 텍스트/결과 패널/공유 버튼 연결)
   - `ShareManager` (캡처 대상 카드/숨길 UI 연결)
   - `FireworkLauncher` 5개 (각 타입 버튼, 쿨다운 이미지, 스폰 위치 연결)
7. ScriptableObject 자동 생성이 필요하면 `Tools > Seoul Firework > Generate Default ScriptableObjects`를 실행한다.

## 2. 게임 실행 및 조작방법
1. 에디터에서 메인 씬을 열고 **Play**를 누른다.
2. 게임은 **180초** 동안 진행되며 자원은 자동 회복된다.
3. 하단/측면의 불꽃 버튼(5종)을 눌러 발사한다.
   - 자원이 부족하거나 쿨다운 중이면 발사되지 않는다.
   - 버튼의 Radial Fill로 쿨다운 진행 상태를 확인한다.
4. 콤보 조건을 시간 내 충족하면 보너스 관중/만족도가 즉시 적용되고 토스트가 표시된다.
5. 종료 후 결과 패널에서 점수/등급/칭호를 확인하고 공유 버튼으로 결과 카드를 저장한다.
6. WebGL 빌드는 `Tools > Seoul Firework > Build WebGL` 실행 후 `Build/WebGL` 폴더 산출물을 정적 호스팅에 업로드해 웹에서 실행한다.