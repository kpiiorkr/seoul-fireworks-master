# AGENT 운영 지침 (Unity Seoul Fireworks)

## 1. 기술 기준
- Unity 버전: **Unity 2022.3 LTS 이상**
- 렌더 파이프라인: URP 2D
- 언어/런타임: C# / .NET Standard(Unity 기본)

## 2. 코딩 컨벤션
- 인스펙터 노출 필드는 반드시 `[SerializeField] private` 패턴을 사용한다.
- 런타임 상태 접근은 `public` 필드 대신 `public` 읽기 전용 프로퍼티 또는 메서드로 제공한다.
- `MonoBehaviour` 간 결합은 이벤트 기반으로 느슨하게 유지한다.
- Null 허용 참조는 시작 시점(`Awake`/`Start`)에 검증하고 오류를 명확히 로그로 남긴다.

## 3. UI 표준
- 텍스트 UI는 반드시 **TextMeshPro (`TextMeshProUGUI`)**를 사용한다.
- Canvas Scaler는 **16:9 기준 스케일**로 설정하고, 모바일/PC 양쪽 해상도에서 동일한 정보 계층을 유지한다.
- 콤보 토스트, 결과 패널, 공유 버튼은 모두 Canvas 상에서 앵커 기반 레이아웃으로 구성한다.

## 4. 프로젝트 경로 규약
- 스크립트: `Assets/Scripts/`
- 아트 리소스: `Assets/Art/`
- ScriptableObject 에셋: `Assets/ScriptableObjects/`
- SVG 아이콘: `Assets/Art/SVG/Icons/`

## 5. 아키텍처 규약
- 게임 상태의 단일 소스는 `GameManager`다.
- `FireworkLauncher`는 발사 요청과 연출 트리거만 담당한다.
- `UIManager`는 렌더링/표시 계층으로 유지하고 게임 계산 로직을 포함하지 않는다.
- `ShareManager`는 캡처와 저장 책임만 가진다.

## 6. 품질 규약
- TODO/미완성 주석 없이 즉시 실행 가능한 코드만 반영한다.
- 불꽃/콤보 수치는 PRD의 원본 수치를 그대로 사용한다.
- 씬 연결이 누락되면 플레이가 멈추므로 필수 참조는 모두 인스펙터에서 직렬화 필드로 노출한다.
