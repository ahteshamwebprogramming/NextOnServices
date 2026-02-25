import { useState } from 'react';
import { Menu, FileText, Upload, Search, Download, FileSpreadsheet, ChevronRight, Flag, ChevronsUpDown, Check } from 'lucide-react';
import { Button } from './components/ui/button';
import { Input } from './components/ui/input';
import { Checkbox } from './components/ui/checkbox';
import { Popover, PopoverContent, PopoverTrigger } from './components/ui/popover';
import { Command, CommandEmpty, CommandGroup, CommandInput, CommandItem, CommandList } from './components/ui/command';
import { cn } from './components/ui/utils';

interface Project {
  pno: string;
  pname: string;
  client: string;
  pm: string;
  country: string;
  loi: number;
  cpi: string;
  irate: string;
  status: string;
  total: number;
  co: number;
  tr: number;
  oq: number;
  st: number;
  fe: number;
  ic: number;
  ir: string;
}

const mockProjects: Project[] = [
  { pno: 'NXT217912', pname: 'New GRP Test', client: 'NextON', pm: 'admin', country: 'ALL Countries', loi: 5, cpi: '$0', irate: '50%', status: 'Awarded', total: 14, co: 6, tr: 1, oq: 0, st: 0, fe: 0, ic: 7, ir: '85%' },
  { pno: 'NXT214843', pname: 'abc-test1', client: 'ABD', pm: 'admin', country: 'IN1', loi: 1, cpi: '$1', irate: '1%', status: 'Awarded', total: 0, co: 0, tr: 0, oq: 0, st: 0, fe: 0, ic: 0, ir: '0%' },
  { pno: 'NXT214670', pname: '1083647', client: '', pm: 'admin', country: 'USA', loi: 5, cpi: '$3.75', irate: '65%', status: 'Awarded', total: 0, co: 0, tr: 0, oq: 0, st: 0, fe: 0, ic: 0, ir: '0%' },
  { pno: 'NXT214506', pname: '1046534', client: '', pm: 'admin', country: 'USA', loi: 1, cpi: '$7.5', irate: '1%', status: 'Awarded', total: 9, co: 0, tr: 0, oq: 0, st: 0, fe: 0, ic: 0, ir: '0%' },
  { pno: 'NXT214505', pname: '1046535', client: '', pm: 'admin', country: 'USA', loi: 1, cpi: '$7.5', irate: '1%', status: 'Awarded', total: 0, co: 0, tr: 0, oq: 0, st: 0, fe: 0, ic: 0, ir: '0%' },
  { pno: 'NXT214503', pname: '1046467', client: '', pm: 'admin', country: 'USA', loi: 25, cpi: '$8.5', irate: '70%', status: 'Awarded', total: 2, co: 0, tr: 0, oq: 0, st: 0, fe: 0, ic: 2, ir: '0%' },
  { pno: 'NXT213839', pname: 'SBOL_INV112020_NOS_Single', client: 'INN', pm: 'admin', country: 'GBR', loi: 20, cpi: '$10', irate: '70%', status: 'Awarded', total: 1, co: 0, tr: 0, oq: 0, st: 0, fe: 0, ic: 1, ir: '0%' },
  { pno: 'NXT213810', pname: 'Training', client: 'INN', pm: 'admin', country: 'ALL Countries', loi: 20, cpi: '$24', irate: '40%', status: 'Awarded', total: 1, co: 1, tr: 0, oq: 0, st: 0, fe: 0, ic: 0, ir: '100%' },
];

export default function App() {
  const [searchQuery, setSearchQuery] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [sidebarOpen, setSidebarOpen] = useState(true);
  const [blueFlag, setBlueFlag] = useState(false);
  const [yellowFlag, setYellowFlag] = useState(false);
  const [redFlag, setRedFlag] = useState(false);
  const [openPM, setOpenPM] = useState(false);
  const [openStatus, setOpenStatus] = useState(false);
  const [selectedPM, setSelectedPM] = useState('admin');
  const [selectedStatus, setSelectedStatus] = useState('--Select--');

  const pmOptions = [
    { value: 'all', label: 'All' },
    { value: 'admin', label: 'admin' },
    { value: 'client-review', label: 'Client Review' },
    { value: 'nexton3', label: 'Nexton3' },
  ];

  const statusOptions = [
    { value: 'select', label: '--Select--' },
    { value: 'awarded', label: 'Awarded' },
    { value: 'pending', label: 'Pending' },
    { value: 'completed', label: 'Completed' },
  ];

  const stats = [
    { label: 'Total Projects', value: '8', icon: FileText },
    { label: 'Active Projects', value: '8', icon: FileText },
    { label: 'Inactive Projects', value: '0', icon: FileText },
    { label: 'Archived Projects', value: '13', icon: FileText },
  ];

  return (
    <div className="flex h-screen bg-slate-50">
      {/* Sidebar */}
      <aside className={`bg-white border-r border-slate-200 transition-all duration-300 ${sidebarOpen ? 'w-48' : 'w-0'} overflow-hidden`}>
        <div className="p-3 border-b border-slate-200">
          <div className="flex items-center gap-2">
            <div className="w-7 h-7 bg-teal-600 rounded flex items-center justify-center">
              <span className="text-white text-xs font-bold">NS</span>
            </div>
            <div>
              <div className="text-xs font-semibold text-slate-900">NextOn</div>
              <div className="text-[10px] text-slate-500">Services</div>
            </div>
          </div>
        </div>
        <nav className="p-2">
          <button className="w-full flex items-center gap-2 px-2 py-1.5 rounded bg-teal-600 text-white text-xs">
            <FileText className="w-3.5 h-3.5" />
            Dashboard
          </button>
          <button className="w-full flex items-center justify-between gap-2 px-2 py-1.5 rounded hover:bg-slate-50 text-slate-700 text-xs mt-1">
            <div className="flex items-center gap-2">
              <FileText className="w-3.5 h-3.5" />
              Projects
            </div>
            <ChevronRight className="w-3 h-3" />
          </button>
          <button className="w-full flex items-center justify-between gap-2 px-2 py-1.5 rounded hover:bg-slate-50 text-slate-700 text-xs mt-1">
            <div className="flex items-center gap-2">
              <FileText className="w-3.5 h-3.5" />
              API Projects
            </div>
            <ChevronRight className="w-3 h-3" />
          </button>
          <button className="w-full flex items-center justify-between gap-2 px-2 py-1.5 rounded hover:bg-slate-50 text-slate-700 text-xs mt-1">
            <div className="flex items-center gap-2">
              <FileText className="w-3.5 h-3.5" />
              Supplier
            </div>
            <ChevronRight className="w-3 h-3" />
          </button>
          <button className="w-full flex items-center justify-between gap-2 px-2 py-1.5 rounded hover:bg-slate-50 text-slate-700 text-xs mt-1">
            <div className="flex items-center gap-2">
              <FileText className="w-3.5 h-3.5" />
              Client
            </div>
            <ChevronRight className="w-3 h-3" />
          </button>
          <button className="w-full flex items-center justify-between gap-2 px-2 py-1.5 rounded hover:bg-slate-50 text-slate-700 text-xs mt-1">
            <div className="flex items-center gap-2">
              <FileText className="w-3.5 h-3.5" />
              Questionnaire
            </div>
            <ChevronRight className="w-3 h-3" />
          </button>
        </nav>
      </aside>

      {/* Main Content */}
      <div className="flex-1 flex flex-col overflow-hidden">
        {/* Header */}
        <header className="bg-white border-b border-slate-200 px-4 py-2">
          <div className="flex items-center justify-between">
            <div className="flex items-center gap-3">
              <button onClick={() => setSidebarOpen(!sidebarOpen)}>
                <Menu className="w-4 h-4 text-slate-600" />
              </button>
            </div>
            <div className="text-xs text-slate-600">
              Welcome <span className="font-semibold">Admin</span>
            </div>
          </div>
        </header>

        {/* Dashboard Content */}
        <main className="flex-1 overflow-auto p-4">
          <div className="max-w-[1600px] mx-auto">
            {/* Page Title */}
            <div className="mb-3">
              <h1 className="text-base font-semibold text-slate-900">Dashboard</h1>
              <p className="text-xs text-slate-500">Project overview and list</p>
            </div>

            {/* Stats Cards - Compact Version */}
            <div className="grid grid-cols-4 gap-3 mb-4">
              {stats.map((stat, index) => (
                <div key={index} className="bg-teal-600 rounded-lg p-3 text-white">
                  <div className="flex items-start justify-between">
                    <div>
                      <div className="text-xl font-bold mb-0.5">{stat.value}</div>
                      <div className="text-xs text-teal-100">{stat.label}</div>
                    </div>
                    <stat.icon className="w-5 h-5 text-teal-200" />
                  </div>
                </div>
              ))}
            </div>

            {/* Project List */}
            <div className="bg-white rounded-lg border border-slate-200">
              {/* Section Header */}
              <div className="border-b border-slate-200 px-4 py-2 flex items-center">
                <div className="w-1 h-4 bg-teal-600 mr-2 rounded"></div>
                <h2 className="text-sm font-semibold text-slate-900">Project List</h2>
              </div>

              {/* Filters */}
              <div className="px-4 py-2.5 border-b border-slate-200 flex items-center gap-2">
                <Popover open={openPM} onOpenChange={setOpenPM}>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      role="combobox"
                      aria-expanded={openPM}
                      className="w-32 h-8 text-xs bg-teal-600 text-white border-teal-600 hover:bg-teal-700 hover:text-white justify-between px-2"
                    >
                      {selectedPM}
                      <ChevronsUpDown className="ml-1 h-3.5 w-3.5 shrink-0 opacity-80" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-[200px] p-0" align="start">
                    <Command>
                      <CommandInput placeholder="Search PM..." className="h-8 text-xs" />
                      <CommandList>
                        <CommandEmpty className="text-xs py-2">No PM found.</CommandEmpty>
                        <CommandGroup>
                          {pmOptions.map((option) => (
                            <CommandItem
                              key={option.value}
                              value={option.value}
                              onSelect={(currentValue) => {
                                setSelectedPM(option.label);
                                setOpenPM(false);
                              }}
                              className="text-xs"
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-3.5 w-3.5",
                                  selectedPM === option.label ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {option.label}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <Popover open={openStatus} onOpenChange={setOpenStatus}>
                  <PopoverTrigger asChild>
                    <Button
                      variant="outline"
                      role="combobox"
                      aria-expanded={openStatus}
                      className="w-32 h-8 text-xs bg-teal-600 text-white border-teal-600 hover:bg-teal-700 hover:text-white justify-between px-2"
                    >
                      {selectedStatus}
                      <ChevronsUpDown className="ml-1 h-3.5 w-3.5 shrink-0 opacity-80" />
                    </Button>
                  </PopoverTrigger>
                  <PopoverContent className="w-[200px] p-0" align="start">
                    <Command>
                      <CommandInput placeholder="Search..." className="h-8 text-xs" />
                      <CommandList>
                        <CommandEmpty className="text-xs py-2">No results found.</CommandEmpty>
                        <CommandGroup>
                          {statusOptions.map((option) => (
                            <CommandItem
                              key={option.value}
                              value={option.value}
                              onSelect={(currentValue) => {
                                setSelectedStatus(option.label);
                                setOpenStatus(false);
                              }}
                              className="text-xs"
                            >
                              <Check
                                className={cn(
                                  "mr-2 h-3.5 w-3.5",
                                  selectedStatus === option.label ? "opacity-100" : "opacity-0"
                                )}
                              />
                              {option.label}
                            </CommandItem>
                          ))}
                        </CommandGroup>
                      </CommandList>
                    </Command>
                  </PopoverContent>
                </Popover>
                <div className="flex gap-1 ml-1">
                  <label className="flex items-center gap-1 px-2 py-1 border border-slate-200 rounded hover:bg-slate-50 cursor-pointer">
                    <Checkbox 
                      checked={blueFlag}
                      onCheckedChange={(checked) => setBlueFlag(checked as boolean)}
                      className="data-[state=checked]:bg-blue-600 data-[state=checked]:border-blue-600"
                    />
                    <Flag className="w-3.5 h-3.5 text-blue-600 fill-blue-600" />
                  </label>
                  <label className="flex items-center gap-1 px-2 py-1 border border-slate-200 rounded hover:bg-slate-50 cursor-pointer">
                    <Checkbox 
                      checked={yellowFlag}
                      onCheckedChange={(checked) => setYellowFlag(checked as boolean)}
                      className="data-[state=checked]:bg-yellow-500 data-[state=checked]:border-yellow-500"
                    />
                    <Flag className="w-3.5 h-3.5 text-yellow-500 fill-yellow-500" />
                  </label>
                  <label className="flex items-center gap-1 px-2 py-1 border border-slate-200 rounded hover:bg-slate-50 cursor-pointer">
                    <Checkbox 
                      checked={redFlag}
                      onCheckedChange={(checked) => setRedFlag(checked as boolean)}
                      className="data-[state=checked]:bg-red-600 data-[state=checked]:border-red-600"
                    />
                    <Flag className="w-3.5 h-3.5 text-red-600 fill-red-600" />
                  </label>
                </div>
              </div>

              {/* Export Buttons */}
              <div className="px-4 py-2 flex items-center justify-between">
                <div className="flex items-center gap-2">
                  <button className="flex items-center gap-1.5 px-2.5 py-1 bg-green-600 text-white rounded text-xs hover:bg-green-700">
                    <FileSpreadsheet className="w-3 h-3" />
                    Excel
                  </button>
                  <button className="flex items-center gap-1.5 px-2.5 py-1 bg-slate-600 text-white rounded text-xs hover:bg-slate-700">
                    <FileText className="w-3 h-3" />
                    CSV
                  </button>
                </div>
                <div className="flex items-center gap-2">
                  <span className="text-xs text-slate-600">Search:</span>
                  <Input 
                    type="text" 
                    className="w-40 h-8 text-xs"
                    value={searchQuery}
                    onChange={(e) => setSearchQuery(e.target.value)}
                  />
                </div>
              </div>

              {/* Table */}
              <div className="overflow-x-auto">
                <table className="w-full">
                  <thead>
                    <tr className="bg-teal-700 text-white text-[11px]">
                      <th className="px-3 py-1.5 text-left font-semibold">PNo</th>
                      <th className="px-3 py-1.5 text-left font-semibold">PName</th>
                      <th className="px-3 py-1.5 text-left font-semibold">Client</th>
                      <th className="px-3 py-1.5 text-left font-semibold">PM</th>
                      <th className="px-3 py-1.5 text-left font-semibold">Country</th>
                      <th className="px-3 py-1.5 text-left font-semibold">LOI</th>
                      <th className="px-3 py-1.5 text-left font-semibold">CPI</th>
                      <th className="px-3 py-1.5 text-left font-semibold">iRate</th>
                      <th className="px-3 py-1.5 text-left font-semibold">Status</th>
                      <th className="px-3 py-1.5 text-left font-semibold">Total</th>
                      <th className="px-3 py-1.5 text-left font-semibold">CO</th>
                      <th className="px-3 py-1.5 text-left font-semibold">TR</th>
                      <th className="px-3 py-1.5 text-left font-semibold">OQ</th>
                      <th className="px-3 py-1.5 text-left font-semibold">ST</th>
                      <th className="px-3 py-1.5 text-left font-semibold">FE</th>
                      <th className="px-3 py-1.5 text-left font-semibold">IC</th>
                      <th className="px-3 py-1.5 text-left font-semibold">IR</th>
                    </tr>
                  </thead>
                  <tbody>
                    {mockProjects.slice((currentPage - 1) * 8, currentPage * 8).map((project, index) => (
                      <tr key={project.pno} className={`border-b border-slate-100 text-[11px] ${index % 2 === 0 ? 'bg-white' : 'bg-slate-50'} hover:bg-blue-50`}>
                        <td className="px-3 py-1.5 text-teal-700 font-medium">{project.pno}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.pname}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.client}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.pm}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.country}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.loi}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.cpi}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.irate}</td>
                        <td className="px-3 py-1.5">
                          <span className="px-2 py-0.5 bg-blue-500 text-white rounded-full text-[10px]">
                            {project.status}
                          </span>
                        </td>
                        <td className="px-3 py-1.5 text-slate-700">{project.total}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.co}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.tr}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.oq}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.st}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.fe}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.ic}</td>
                        <td className="px-3 py-1.5 text-slate-700">{project.ir}</td>
                      </tr>
                    ))}
                  </tbody>
                </table>
              </div>

              {/* Pagination */}
              <div className="px-4 py-2 border-t border-slate-200 flex items-center justify-between">
                <div className="text-[11px] text-slate-600">
                  Showing 1 to 8 of 8 entries
                </div>
                <div className="flex items-center gap-1.5">
                  <Button 
                    variant="outline" 
                    size="sm" 
                    className="h-7 text-[11px] px-2.5"
                    onClick={() => setCurrentPage(Math.max(1, currentPage - 1))}
                    disabled={currentPage === 1}
                  >
                    Previous
                  </Button>
                  <Button variant="outline" size="sm" className="h-7 w-7 text-[11px] bg-slate-100">
                    1
                  </Button>
                  <Button 
                    variant="outline" 
                    size="sm" 
                    className="h-7 text-[11px] px-2.5"
                    onClick={() => setCurrentPage(currentPage + 1)}
                  >
                    Next
                  </Button>
                </div>
              </div>
            </div>

            {/* Footer */}
            <div className="mt-4 text-center text-[11px] text-slate-500">
              Copyright © 2024. NextOn Services. All rights reserved.
            </div>
          </div>
        </main>
      </div>
    </div>
  );
}