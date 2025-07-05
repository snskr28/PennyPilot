import { DashboardFilter } from "./dashboard-filter.model";

export interface TableRequest {
    pageNumber: number;
    pageSize: number;
    sortBy?: string;
    sortOrder?: 'asc' | 'desc';
    dashboardFilter? : DashboardFilter;
}