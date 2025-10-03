## TEST PLAN — DemoProductApi

เอกสารนี้เป็นแผนการทดสอบระดับสูงสำหรับระบบ DemoProductApi (API สำหรับจัดการสินค้า, ราคา, สต็อก, สถานที่ และ bundle)
เนื้อหาเขียนเป็นภาษาไทยและเน้นแปลงเป็นงานที่ทำได้จริง (actionable).

---

## 1. Scope

- ระบบภายใต้ขอบเขต: Backend API ของ `DemoProductApi` (โครงการในโฟลเดอร์ `DemoProductApi/`) ซึ่งรวมถึง endpoints ที่นิยามใน:
  - `Controllers/*` (เช่น `ProductController`, `BundleController`, `InventoryController`, `PriceController`, `LocationController`, `ProductItemController`)
  - บริการใน `DemoProductApi.Application/Services/*` และ repository/DB layer ใน `DemoProductApi.Infrastructure/`
- ประเภทการทดสอบที่จะทำ: Unit tests, API tests (contract/functional), Integration tests (DB, external resources), End-to-End (E2E) test สำคัญ, Non-functional tests (performance / load / security smoke checks)
- Test data: ใช้ seed scripts หรือ fixtures ใน `DemoProductApi.Infrastructure/Seed` (ถ้ามี) และ test-specific fixtures

## 2. Out-of-scope

- UI (ไม่มี frontend ใน repo)
- Third-party integrations ที่ไม่ถูกติดตั้งหรือไม่สามารถจำลองได้ (เช่น external payment gateway) — จะ mock/stub แทน
- Long-running performance benchmarking (ถ้าต้องการ จะกำหนดเป็น project แยก)

## 3. Risks & Assumptions

- Risks
  - DB schema drift ระหว่าง migrations กับ seed data → ทำให้ integration tests ล้มเหลว
  - Flaky tests จากเวลาหรือการพึ่งพาเวลา/ระบบภายนอก
  - Missing test data or inconsistent seed across envs
- Assumptions
  - โค้ดสามารถรัน `dotnet test` ในเครื่อง dev/CI
  - มี migrations / seed scripts ใน `DemoProductApi.Infrastructure/Migrations` หรือ `Seed` folder
  - CI pipeline สามารถสร้าง container หรือ database test instance (Docker or ephemeral DB)

## 4. Test Levels

### 4.1 Unit Tests

- สิ่งที่ต้องทดสอบ: services (`DemoProductApi.Application/Services/*`), DTO mappers (`*Mapper.cs`), validators (`Validators/*`)
- เทคนิค: mock repositories/interfaces (`Interfaces/Repositories/*`) using a mocking framework (Moq / NSubstitute)
- Acceptance criteria: fast (<100ms per test), deterministic, no external I/O
- Targets: cover happy-path + edge cases (null, empty, invalid input)

Actionable checklist:

- [ ] เพิ่ม/อัปเดต Unit tests สำหรับแต่ละ service: happy, error, boundary
- [ ] ใช้ In-memory or mocked dependencies only

### 4.2 API Tests (Contract / Functional)

- สิ่งที่ต้องทดสอบ: HTTP endpoints for CRUD and business flows
- เทคนิค: spin up TestServer (Microsoft.AspNetCore.TestHost) หรือ run in-memory WebApplicationFactory
## TEST PLAN — DemoProductApi

เอกสารนี้สรุปแนวทางการทดสอบสำหรับบริการ Backend ของ DemoProductApi (API จัดการสินค้า, ราคา, สต็อก, สถานที่ และ bundle) โดยมุ่งเน้นการปฏิบัติที่ชัดเจนและตรวจวัดผลได้

---

## 1. ขอบเขต (Scope)

- ระบบ: Backend API ในโฟลเดอร์ `DemoProductApi/` รวม Controllers (`DemoProductApi/Controllers/*`), Application services (`DemoProductApi.Application/Services/*`) และชั้น Infrastructure/DB (`DemoProductApi.Infrastructure/*`).
- ประเภทการทดสอบ: Unit, API (contract/functional), Integration (DB), End-to-End (E2E) และ Non-functional (performance, basic security).

## 2. ขอบเขตที่ไม่รวม (Out-of-scope)

- Frontend/UI (repo นี้ไม่มี)
- Third-party services ที่ไม่สามารถจำลองได้ — ให้ mock หรือ stub แทน
- การทดสอบประสิทธิภาพระยะยาวหรือ benchmarking ขนาดใหญ่ (จะแยกเป็นงานต่างหาก)

## 3. ความเสี่ยงและสมมติฐาน

- ความเสี่ยงหลัก: schema drift, flaky tests, ข้อมูลทดสอบไม่สอดคล้อง
- สมมติฐาน: โค้ดรัน `dotnet test` ได้, มี migrations/seed scripts, CI สามารถสร้าง DB ephemeral

## 4. ระดับการทดสอบ (Test Levels)

- Unit: ทดสอบ services, mappers, validators ด้วย mocking (เป้าหมาย: เร็ว, deterministic)
- API: ทดสอบ contract และพฤติกรรมของ HTTP endpoints โดยใช้ `WebApplicationFactory`/TestServer
- Integration: ใช้ DB ephemeral, apply migrations และ seed ข้อมูลเล็กน้อย
- E2E: รัน flows สำคัญบน staging-like environment
- Non-functional: smoke tests สำหรับ latency/throughput และ basic security checks

## 5. Environment & Data Strategy

Environment (สรุป):

- Local: macOS (zsh), .NET SDK ตามโปรเจค, DB สำหรับ integration via Docker (Postgres/SQL Server) หรือ SQLite สำหรับ run เร็ว
- CI: Linux runner (GitHub Actions), DB ephemeral (Docker), ขั้นตอน: checkout → setup-dotnet → start DB → migrations → seed → tests → collect artifacts
- Staging: isolated staging DB, seeded ด้วยข้อมูลตัวอย่างที่ไม่เปิดเผย

Supported browsers (สำหรับ E2E ที่อาจใช้ UI automation): Chrome (หลัก), Firefox (สำรอง)

Test data formats & locations:

- Account fixtures: `tests/fixtures/accounts/` (JSON)
- Seed scripts: `DemoProductApi.Infrastructure/Seed/` หรือ `scripts/seed/` (SQL หรือ EF Core seeder)
- Test Case definitions: `tests/cases/` (YAML/JSON) — โครง: id, title, preconditions, steps, expected

Credentials: เก็บเฉพาะตัวอย่างใน `tests/fixtures/credentials.example.env`; ค่าจริงเก็บเป็น CI secrets / env vars (`TEST_DB_CONNECTION`, `TEST_API_KEY_ADMIN`)

Actionables:

- จัดทำ `docker-compose.test.yml` สำหรับ CI
- สร้าง `tests/fixtures/credentials.example.env` และตัวอย่าง fixture
- เก็บ test cases ใต้ `tests/cases/` เป็น YAML/JSON

## 6. Entry / Exit Criteria

- Entry (PR): คอมไพล์ผ่าน, Unit tests ผ่าน, migrations/seed อยู่ใน PR
- Exit (merge/release): Unit & API tests ผ่าน, E2E smoke ใน staging ผ่าน, ไม่มี P0/P1 ค้าง, coverage ตามเป้า

## 7. Metrics

- ติดตาม: pass rate, code coverage (line/branch), API coverage, defect trends
- เป้าหมายตัวอย่าง: Unit 100% pass, Integration/API >=98% pass, Coverage >=80%

## 8. Execution & Reporting

- CI: รัน Unit ทุก PR; Integration/E2E บน main หรือ nightly
- รายงาน: เก็บ XML test results, coverage report; post summary บน PR และช่องทางทีม

## 9. Schedule & Ownership

- ตัวอย่าง timeline 3 สัปดาห์ (planning → tests → stabilization → E2E & sign-off)
- ระบุความรับผิดชอบ: QA Lead, Automation Engineer (SDET), Dev Owner, Release Manager

## 10. Appendix

- โครงสร้างสำคัญใน repo:
  - `DemoProductApi/Controllers/*`
  - `DemoProductApi.Application/Services/*`
  - `DemoProductApi/Validators/*`
  - `DemoProductApi.Infrastructure/*`
- คำสั่งตัวอย่าง (local):

```bash
dotnet test ./DemoProductApi.Tests/DemoProductApi.Tests.csproj --collect:"XPlat Code Coverage"
cd DemoProductApi
dotnet run
```

---

## Traceability: Feature ↔ Test Type ↔ Artifact

| Feature | Test Type(s) | Artifact / Location |
|---|---|---|
| Product Management | Unit, API, Integration, E2E | Unit tests: `DemoProductApi.Tests/Unit/Product*`<br>API tests: `DemoProductApi.Tests/Api/Product*`<br>Seed: `DemoProductApi.Infrastructure/Seed/` |
| Inventory Management | Unit, Integration, E2E | Unit tests: `.../Inventory*`<br>Integration tests: `DemoProductApi.Tests/Integration/Inventory*`<br>E2E flows: `tests/e2e/inventory` |
| Pricing (ProductItem/Bundle) | Unit, API, Integration, Non-functional | Unit tests: `.../Price*`<br>API tests: `DemoProductApi.Tests/Api/Price*`<br>k6 script: `tests/perf/k6_price_smoke.js` |
| Bundles & Reservations | Unit, API, Integration, E2E | Unit tests: `.../Bundle*`<br>API tests: `DemoProductApi.Tests/Api/Bundle*`<br>E2E: `tests/e2e/bundle` |
| Security / Auth | Unit, API, E2E | API tests validating 401/403: `.../Auth*`<br>E2E auth scenarios: `tests/e2e/security` |

Notes: ปรับ path ตามโครง repo หลังจากสร้างโฟลเดอร์ทดสอบจริง

## Next steps — คำแนะนำงานถัดไป (Actionable)

1) Unit tests (priority: สูง)
  - สร้าง/อัปเดตโครง Unit test ตาม feature owners
  - เขียน tests: happy path, validation, edge cases, exceptions
  - Integrate coverlet และให้ CI รายงาน coverage
  - Deliverable: ทุก service สำคัญมี Unit tests และ coverage report

2) E2E smoke (priority: กลาง)
  - กำหนด 5-10 critical flows (จาก Traceability table)
  - สร้าง E2E scripts รันบน staging-like environment (ใช้ Playwright/HTTP orchestration)
  - Ensure seed/reset scripts พร้อมก่อนรัน
  - Deliverable: E2E smoke suite ที่รันอัตโนมัติใน CI/staging

3) Reporting & CI automation (priority: กลาง)
  - สร้าง GitHub Actions workflow: jobs สำหรับ Unit, Integration, E2E, publish artifacts
  - เปิดการเก็บและเผยแพร่ coverage (Codecov/SonarCloud) และ test result trends
  - Deliverable: CI pipeline ที่ให้รายงานผลและ artifact link ใน PR

---

หากต้องการ ผมสามารถสร้างตัวอย่างไฟล์ต่อไปนี้ให้ทันที: `/.github/workflows/ci.yml`, `docker-compose.test.yml`, ตัวอย่าง fixture และตัวอย่าง E2E script — ระบุสิ่งที่ต้องการให้เริ่มก่อน


- Week 0 (planning): finalize test scenarios, update seed/migrations
- Week 1: implement unit + API tests for new features
- Week 2: implement integration tests; stabilize CI
- Week 3: run E2E + non-functional smoke; final sign-off

Roles & ownership (suggested):

| Role | Responsibility | Owner (example) |
|---|---|---|
| QA Lead | Test plan, release gate decision | (QA Lead) |
| Automation Engineer | Implement CI test jobs, E2E scripts | (SDET) |
| Dev Owner | Review & fix failing tests, maintain mocks | (Feature dev) |
| Release Manager | Trigger release, validate Exit Criteria | (Release) |

Actionable onboarding:

- [ ] Assign owners for each controller/service to maintain tests
- [ ] Add test ownership badge or file `TEST_OWNERS.md` linking files to owners

## 10. Appendix

- Repo references to help writing tests:
  - Controllers: `DemoProductApi/Controllers/*`
  - Application services: `DemoProductApi.Application/Services/*`
  - Validators: `DemoProductApi/Validators/*`
  - Infrastructure and DB: `DemoProductApi.Infrastructure/*` (migrations, seed)
- Helpful commands (local):

  - Run unit tests and coverage locally (example):

  ```bash
  dotnet test ./DemoProductApi.Tests/DemoProductApi.Tests.csproj --collect:"XPlat Code Coverage"
  ```

  - Run API locally (development profile):

  ```bash
  cd DemoProductApi
  dotnet run
  ```

- Example test checklist (pre-merge):

- [ ] Unit tests: all green
- [ ] API contract tests: endpoints exercised for new/changed APIs
- [ ] Integration tests: migrations applied and queries validated
- [ ] E2E smoke (if release): critical flows green on staging

---

Ifต้องการ ผม/ฉันสามารถ: สร้างตัวอย่างไฟล์ CI job (GitHub Actions) ที่รัน unit/integration/E2E, หรือสร้าง `docker-compose.test.yml` + small scripts เพื่อรัน migrations และ seed data อัตโนมัติ — บอกได้ว่าต้องการแบบใดได้เลย

Additional artifacts suggested (actionable):

- `tests/fixtures/credentials.example.env` — documents required env vars for tests
- `tests/cases/` — store YAML/JSON test case definitions
- `scripts/seed/run_seed.sh` — bootstrap seed runner (shell script)