window.downloadCsv = (filename, base64) => {
    const a = document.createElement('a');
    a.href = 'data:text/csv;charset=utf-8;base64,' + base64;
    a.download = filename;
    document.body.appendChild(a);
    a.click();
    document.body.removeChild(a);
};
