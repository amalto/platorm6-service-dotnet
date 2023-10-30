function () {
    del = document.getElementById('gridDiv')
    if (null !== del) {
        del.remove()
    }
    d = document.createElement('div')
    d.setAttribute('id', 'gridDiv')
    document.body.append(d)

    css = document.createElement('link')
    css.href = 'https://cdnjs.cloudflare.com/ajax/libs/extjs/6.2.0/classic/theme-crisp-touch/resources/theme-crisp-touch-all.css'
    css.rel = 'stylesheet'
    css.type = 'text/css'
    document.body.appendChild(css)

    script = document.createElement('script')
    script.src = 'https://cdnjs.cloudflare.com/ajax/libs/extjs/6.2.0/ext-all.js'
    document.body.appendChild(script)

    script.addEventListener('load', (event) => {
        loadGrid()
    })
}

function loadGrid() {

    // Creation of data model
    Ext.define('CryptoDataModel', {
        extend: 'Ext.data.Model',
        fields: [
            { name: 'id', mapping: 'id' },
            { name: 'rank', mapping: 'rank' },
            { name: 'symbol', mapping: 'symbol' },
            { name: 'name', mapping: 'name' },
            { name: 'supply', mapping: 'supply' },
            { name: 'maxSupply', mapping: 'maxSupply' },
            { name: 'marketCapUsd', mapping: 'marketCapUsd' },
            { name: 'volumeUsd24Hr', mapping: 'volumeUsd24Hr' },
            { name: 'priceUsd', mapping: 'priceUsdm' },
            { name: 'changePercent24Hr', mapping: 'changePercent24Hr' },
            { name: 'vwap24Hr', mapping: 'vwap24Hr' }
        ]
    });

    Ext.onReady(function () {

        var gridStore = Ext.create('Ext.data.Store', {
            model: 'CryptoDataModel',
            proxy: {
                type: 'ajax',
                url: 'https://devproxy.amalto.io/crypto?&p6proxyNoToken&baseUrl=http://dev.internal.sidetrade.io:9192',
                reader: {
                    type: 'json',
                    rootProperty: 'data'
                }
            },
            autoLoad: true
        });

        Ext.create('Ext.grid.Panel', {
            id: 'gridId',
            store: gridStore,
            stripeRows: true,
            title: 'Cryptocurrency',
            renderTo: 'gridDiv',
            width: '100%',
            collapsible: true,
            enableColumnMove: true,
            enableColumnResize: true,

            columns:
                [{
                    header: "ID",
                    dataIndex: 'id',
                    id: 'id',
                    flex: .75,
                    sortable: true,
                    hideable: true
                }, {
                    header: "Rank",
                    dataIndex: 'rank',
                    flex: .5,
                    sortable: true,
                    hideable: false
                }, {
                    header: "Symbol",
                    dataIndex: 'symbol',
                    flex: .5,
                    sortable: true
                }, {
                    header: "Name",
                    dataIndex: 'name',
                    flex: .5,
                    sortable: true
                }, {
                    header: "Supply",
                    dataIndex: 'supply',
                    flex: .5,
                    sortable: true
                }, {
                    header: "MaxSupply",
                    dataIndex: 'maxSupply',
                    flex: .5,
                    sortable: true
                }, {
                    header: "Market Cap (USD)",
                    dataIndex: 'marketCapUsd',
                    flex: .5,
                    sortable: true
                }, {
                    header: "Volume USD 24Hr",
                    dataIndex: 'volumeUsd24Hr',
                    flex: .5,
                    sortable: true
                }, {
                    header: "Price (USD)",
                    dataIndex: 'priceUsd',
                    flex: .5,
                    sortable: true
                }, {
                    header: "Change Percent 24Hr",
                    dataIndex: 'changePercent24Hr',
                    flex: .5,
                    sortable: true
                }, {
                    header: "VWAP 24Hr",
                    dataIndex: 'vwap24Hr',
                    flex: .5,
                    sortable: true
                }]
        });

    });
}