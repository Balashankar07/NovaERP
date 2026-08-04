const http = require('http');

const API_URL = 'http://localhost:5232/api';
let superAdminToken = '';
let warehouseId = '';
let locationId = '';

async function makeRequest(method, endpoint, body = null, token = null) {
    return new Promise((resolve, reject) => {
        const url = new URL(`${API_URL}${endpoint}`);
        const options = {
            hostname: url.hostname,
            port: url.port,
            path: url.pathname + url.search,
            method: method,
            headers: {
                'Content-Type': 'application/json',
            }
        };

        if (token) {
            options.headers['Authorization'] = `Bearer ${token}`;
        }

        const req = http.request(options, (res) => {
            let data = '';
            res.on('data', (chunk) => data += chunk);
            res.on('end', () => {
                let parsed = data;
                try {
                    parsed = JSON.parse(data);
                } catch (e) { }
                resolve({ status: res.statusCode, data: parsed });
            });
        });

        req.on('error', (e) => reject(e));

        if (body) {
            req.write(JSON.stringify(body));
        }
        req.end();
    });
}

async function runTests() {
    console.log('--- Starting Warehouse Management Verification ---');
    try {
        const loginRes = await makeRequest('POST', '/Auth/login', {
            email: 'admin@novaerp.com',
            password: 'Admin@123'
        });
        superAdminToken = loginRes.data.data.accessToken;
        console.log("Logged in successfully.");

        // 1. Create Default Warehouse
        console.log("Creating default warehouse...");
        let wh1Res = await makeRequest('POST', '/Warehouses', {
            warehouseCode: 'WH-MAIN',
            warehouseName: 'Main Warehouse',
            isDefault: true
        }, superAdminToken);
        
        console.log(`Create WH-MAIN: ${wh1Res.status}`);
        if(wh1Res.status === 201) {
            warehouseId = wh1Res.data.data.id;
        } else {
            console.error(wh1Res.data);
        }

        // 2. Try creating another default warehouse (should fail or auto-handle)
        // Let's see if the logic blocks it or handles it based on my implementation
        // My implementation: "throw new Exception('Only one default warehouse is allowed.')"
        console.log("Creating second default warehouse (expecting failure)...");
        let wh2Res = await makeRequest('POST', '/Warehouses', {
            warehouseCode: 'WH-SUB',
            warehouseName: 'Sub Warehouse',
            isDefault: true
        }, superAdminToken);
        console.log(`Create second default WH: ${wh2Res.status} (Expected 500 or 400)`);

        // 3. Create non-default warehouse
        console.log("Creating non-default warehouse...");
        let wh3Res = await makeRequest('POST', '/Warehouses', {
            warehouseCode: 'WH-SUB',
            warehouseName: 'Sub Warehouse',
            isDefault: false
        }, superAdminToken);
        console.log(`Create non-default WH: ${wh3Res.status}`);
        let subWarehouseId = wh3Res.data.data.id;

        // 4. Create Location in Main Warehouse
        console.log("Creating location in main warehouse...");
        let loc1Res = await makeRequest('POST', '/WarehouseLocations', {
            warehouseId: warehouseId,
            locationCode: 'A-1',
            locationName: 'Aisle A - Rack 1',
            zone: 'A'
        }, superAdminToken);
        console.log(`Create Location: ${loc1Res.status}`);
        locationId = loc1Res.data.data.id;

        // 5. Duplicate Location Code in same warehouse (should fail)
        console.log("Creating duplicate location code...");
        let loc2Res = await makeRequest('POST', '/WarehouseLocations', {
            warehouseId: warehouseId,
            locationCode: 'A-1',
            locationName: 'Aisle A - Rack 2'
        }, superAdminToken);
        console.log(`Create Duplicate Location Code: ${loc2Res.status} (Expected 500)`);

        // 6. Delete Warehouse containing locations (should fail)
        console.log("Deleting warehouse with locations...");
        let delWhRes = await makeRequest('DELETE', `/Warehouses/${warehouseId}`, null, superAdminToken);
        console.log(`Delete WH with locations: ${delWhRes.status} (Expected 500)`);

        // 7. Test Pagination & Search
        console.log("Testing search...");
        let searchRes = await makeRequest('GET', '/Warehouses?search=Sub', null, superAdminToken);
        console.log(`Search for 'Sub' found: ${searchRes.data.data.items.length} (Expected 1)`);

        // 8. Delete Sub Warehouse (should succeed)
        console.log("Deleting sub warehouse...");
        let delSubRes = await makeRequest('DELETE', `/Warehouses/${subWarehouseId}`, null, superAdminToken);
        console.log(`Delete Sub WH: ${delSubRes.status} (Expected 200)`);

        // 9. Check Audit Log
        console.log("Checking audit logs...");
        let auditRes = await makeRequest('GET', '/AuditLogs?sortBy=timestamp&sortOrder=desc&pageSize=10', null, superAdminToken);
        let whLogs = auditRes.data.data.items.filter(x => x.entityName === 'Warehouse');
        console.log(`Found Audit Logs for Warehouse: ${whLogs.length > 0}`);

        console.log('--- Verification Complete ---');
    } catch (error) {
        console.error("Test execution failed:", error);
    }
}

runTests();
